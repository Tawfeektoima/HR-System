using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HRSystem.Core.DTOs.AI;
using HRSystem.Core.Interfaces;
using HRSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRSystem.Infrastructure.Services;

public class DeepSeekCvComparisonService : ICvComparisonService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DeepSeekCvComparisonService> _logger;

    private const int MaxCvCharsPerCandidate = 14_000;

    public DeepSeekCvComparisonService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorage,
        Microsoft.Extensions.Logging.ILogger<DeepSeekCvComparisonService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<CvComparisonResponseDto> CompareCvScoresForJobAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["DeepSeek:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "DeepSeek:ApiKey is not configured. Add your API key to appsettings.json or user secrets.");

        var job = await _unitOfWork.Jobs.GetByIdAsync(jobId);
        if (job == null)
            throw new InvalidOperationException("Job not found.");

        var applications = await _unitOfWork.Applications.GetByJobIdAsync(jobId);
        if (applications.Count == 0)
            throw new InvalidOperationException("No applications for this job.");

        var blocks = new List<string>();
        foreach (var app in applications)
        {
            var name = app.Candidate.FullName;
            var cvText = await ReadCvTextAsync(app.Candidate.CvFilePath, cancellationToken);
            blocks.Add(
                $"### ApplicationId: {app.Id}\n" +
                $"Candidate: {name}\n" +
                $"CV content:\n{cvText}\n");
        }

        var userPrompt = new StringBuilder();
        userPrompt.AppendLine($"Job title: {job.Title}");
        userPrompt.AppendLine($"Department: {job.Department}");
        userPrompt.AppendLine("Job description:");
        userPrompt.AppendLine(job.Description);
        userPrompt.AppendLine("Requirements:");
        userPrompt.AppendLine(job.Requirements);
        userPrompt.AppendLine();
        userPrompt.AppendLine("Compare all candidates below. Score each from 0 to 100 based on fit to this role.");
        userPrompt.AppendLine("Use the exact ApplicationId values provided. Return JSON only.");
        userPrompt.AppendLine();
        foreach (var b in blocks)
            userPrompt.AppendLine(b);

        var systemPrompt =
            "You are a senior technical recruiter. Compare the CVs for the same job opening. " +
            "Be fair and consistent. Respond with a single JSON object using this schema exactly:\n" +
            "{\"overall_summary\":\"string\",\"scores\":[{\"application_id\":number,\"score\":number,\"reason\":\"string\"}]} " +
            "scores must include every ApplicationId listed. score is 0-100.";

        var model = _configuration["DeepSeek:Model"] ?? "deepseek-chat";
        var http = _httpClientFactory.CreateClient("DeepSeek");

        var requestBody = new DeepSeekChatRequest(
            model,
            new[]
            {
                new DeepSeekMessage("system", systemPrompt),
                new DeepSeekMessage("user", userPrompt.ToString())
            },
            0.2);

        using var response = await http.PostAsJsonAsync(
            "chat/completions",
            requestBody,
            cancellationToken: cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("DeepSeek Raw Response: {Raw}", raw);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"DeepSeek API error: {(int)response.StatusCode} — {raw}");

        var completion = JsonSerializer.Deserialize<DeepSeekChatCompletion>(raw, ChatCompletionJsonOptions);
        var content = completion?.Choices?.FirstOrDefault()?.Message?.Content;
        _logger.LogInformation("DeepSeek Extracted Content: {Content}", content);

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("Empty response from DeepSeek.");

        var jsonPayload = StripMarkdownFence(content);
        var parsed = JsonSerializer.Deserialize<DeepSeekComparisonPayload>(jsonPayload, PayloadJsonOptions);
        if (parsed?.Scores == null || parsed.Scores.Count == 0)
            throw new InvalidOperationException("Could not parse comparison scores from DeepSeek response.");

        var items = new List<CvComparisonScoreItemDto>();
        foreach (var row in parsed.Scores)
        {
            var app = applications.FirstOrDefault(a => a.Id == row.ApplicationId);
            var name = app?.Candidate.FullName ?? $"#{row.ApplicationId}";
            var score = Math.Clamp(row.Score, 0m, 100m);
            items.Add(new CvComparisonScoreItemDto(row.ApplicationId, name, score, row.Reason));

            if (app != null)
            {
                _logger.LogInformation("Updating Application {Id} with score {Score}", app.Id, score);
                app.CvScore = score;
                app.HrNotes = $"AI Comparison ({DateTime.Now:g}): {row.Reason}";
                app.UpdatedAt = DateTime.UtcNow;

                if (app.Candidate != null)
                {
                    _logger.LogInformation("Updating Candidate {Id} TotalScore to {Score}", app.Candidate.Id, score);
                    app.Candidate.TotalScore = score;
                    app.Candidate.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Candidates.UpdateAsync(app.Candidate); // Explicit update
                }

                await _unitOfWork.Applications.UpdateAsync(app);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new CvComparisonResponseDto(
            job.Id,
            job.Title,
            parsed.OverallSummary ?? string.Empty,
            items);
    }

    private async Task<string> ReadCvTextAsync(string? cvPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cvPath))
            return "[No CV file on record]";

        var extension = Path.GetExtension(cvPath).ToLower();
        
        // If it's a plain text file, we can read it directly from storage
        if (extension == ".txt")
        {
            var bytes = await _fileStorage.GetFileAsync(cvPath);
            var text = Encoding.UTF8.GetString(bytes);
            return text.Length > MaxCvCharsPerCandidate ? text[..MaxCvCharsPerCandidate] + "\n...[truncated]" : text;
        }

        // For PDF, DOCX, etc., we use the Python AI service to extract text
        try
        {
            var aiBaseUrl = _configuration["AIServiceUrl"] ?? "http://localhost:8000";
            var httpClient = _httpClientFactory.CreateClient(); 
            
            using var multipartFormContent = new MultipartFormDataContent();
            var fileBytes = await _fileStorage.GetFileAsync(cvPath);
            var fileContent = new ByteArrayContent(fileBytes);
            multipartFormContent.Add(fileContent, "file", Path.GetFileName(cvPath));

            var response = await httpClient.PostAsync($"{aiBaseUrl}/extract-text", multipartFormContent, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);
                var text = result?.RootElement.GetProperty("text").GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Length > MaxCvCharsPerCandidate ? text[..MaxCvCharsPerCandidate] + "\n...[truncated]" : text;
                }
            }
        }
        catch
        {
            // Fallback
        }

        return "[Could not extract text from this file format]";
    }

    private static string StripMarkdownFence(string text)
    {
        var t = text.Trim();
        if (t.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNl = t.IndexOf('\n');
            var lastFence = t.LastIndexOf("```", StringComparison.Ordinal);
            if (firstNl > 0 && lastFence > firstNl)
                t = t[(firstNl + 1)..lastFence].Trim();
        }
        return t;
    }

    private static readonly JsonSerializerOptions ChatCompletionJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly JsonSerializerOptions PayloadJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}

internal sealed record DeepSeekChatRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] DeepSeekMessage[] Messages,
    [property: JsonPropertyName("temperature")] double Temperature);

internal sealed record DeepSeekMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed class DeepSeekChatCompletion
{
    [JsonPropertyName("choices")]
    public List<DeepSeekChoice>? Choices { get; set; }
}

internal sealed class DeepSeekChoice
{
    [JsonPropertyName("message")]
    public DeepSeekMessageBody? Message { get; set; }
}

internal sealed class DeepSeekMessageBody
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

internal sealed class DeepSeekComparisonPayload
{
    [JsonPropertyName("overall_summary")]
    public string? OverallSummary { get; set; }

    [JsonPropertyName("scores")]
    public List<DeepSeekScoreRow>? Scores { get; set; }
}

internal sealed class DeepSeekScoreRow
{
    [JsonPropertyName("application_id")]
    public int ApplicationId { get; set; }

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
