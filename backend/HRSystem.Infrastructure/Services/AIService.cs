using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HRSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace HRSystem.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const int MaxCvChars = 16000;

    public AIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AICvAnalysisResult> AnalyzeCvAsync(string filePath, int jobId, string? jobRequirements = null)
    {
        var deepSeekApiKey = _configuration["DeepSeek:ApiKey"];
        if (!string.IsNullOrWhiteSpace(deepSeekApiKey))
        {
            return await AnalyzeWithDeepSeekAsync(filePath, jobId, jobRequirements, deepSeekApiKey);
        }

        return await AnalyzeWithLegacyAiServiceAsync(filePath, jobId, jobRequirements);
    }

    private async Task<AICvAnalysisResult> AnalyzeWithLegacyAiServiceAsync(string filePath, int jobId, string? jobRequirements)
    {
        var aiBaseUrl = _configuration["AIServiceUrl"] ?? "http://localhost:8000";
        var fileInfo = new FileInfo(filePath);

        using var multipartFormContent = new MultipartFormDataContent();

        var fileStreamContent = new StreamContent(File.OpenRead(filePath));
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileInfo.Name);
        multipartFormContent.Add(new StringContent(jobId.ToString()), name: "job_id");

        if (!string.IsNullOrWhiteSpace(jobRequirements))
        {
            multipartFormContent.Add(new StringContent(jobRequirements), name: "job_requirements");
        }

        var response = await _httpClient.PostAsync($"{aiBaseUrl}/analyze-cv", multipartFormContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"AI Service failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }

        var result = await response.Content.ReadFromJsonAsync<AICvAnalysisResponse>();
        if (result == null)
        {
            throw new Exception("Failed to deserialize AI response");
        }

        return new AICvAnalysisResult(
            result.ExtractedSkills,
            result.ExperienceYears,
            result.EducationLevel,
            result.Score,
            result.Summary
        );
    }

    private async Task<AICvAnalysisResult> AnalyzeWithDeepSeekAsync(
        string filePath,
        int jobId,
        string? jobRequirements,
        string apiKey)
    {
        var baseUrl = _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com/v1/";
        if (!baseUrl.EndsWith('/')) baseUrl += "/";
        var model = _configuration["DeepSeek:Model"] ?? "deepseek-chat";

        var cvText = await ReadCvTextAsync(filePath);
        var requirementsText = string.IsNullOrWhiteSpace(jobRequirements)
            ? "No requirements provided."
            : jobRequirements;

        var systemPrompt =
            "You are an HR screening assistant. Evaluate candidate CV relevance for one job. " +
            "Return only a JSON object with exact shape: " +
            "{\"extracted_skills\":[\"string\"],\"experience_years\":0,\"education_level\":\"string\",\"score\":0,\"summary\":\"string\"}. " +
            "Score must be from 0 to 100.";

        var userPrompt = $"""
JobId: {jobId}
Job requirements:
{requirementsText}

Candidate CV:
{cvText}
""";

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new SingleCvDeepSeekChatRequest(
            model,
            new[]
            {
                new SingleCvDeepSeekMessage("system", systemPrompt),
                new SingleCvDeepSeekMessage("user", userPrompt)
            },
            0.2
        ));

        using var response = await _httpClient.SendAsync(request);
        var raw = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"DeepSeek failed with status code {response.StatusCode}: {raw}");
        }

        var completion = JsonSerializer.Deserialize<SingleCvDeepSeekCompletion>(raw, JsonOpts);
        var content = completion?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new Exception("DeepSeek response content is empty.");
        }

        var json = StripMarkdownFence(content);
        var parsed = JsonSerializer.Deserialize<DeepSeekCvResult>(json, JsonOpts);
        if (parsed == null)
        {
            throw new Exception("Failed to parse DeepSeek response.");
        }

        return new AICvAnalysisResult(
            parsed.ExtractedSkills ?? new List<string>(),
            parsed.ExperienceYears,
            parsed.EducationLevel,
            Math.Clamp(parsed.Score, 0m, 100m),
            parsed.Summary ?? string.Empty
        );
    }

    private async Task<string> ReadCvTextAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        
        // If it's a plain text file, we can read it directly
        if (extension == ".txt")
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            var text = Encoding.UTF8.GetString(bytes);
            return text.Length > MaxCvChars ? text[..MaxCvChars] + "\n...[truncated]" : text;
        }

        // For PDF, DOCX, etc., we use the Python AI service to extract text
        try
        {
            var aiBaseUrl = _configuration["AIServiceUrl"] ?? "http://localhost:8000";
            using var multipartFormContent = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            multipartFormContent.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync($"{aiBaseUrl}/extract-text", multipartFormContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
                var text = result?.RootElement.GetProperty("text").GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Length > MaxCvChars ? text[..MaxCvChars] + "\n...[truncated]" : text;
                }
            }
        }
        catch
        {
            // Fallback to basic string read if something goes wrong
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

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

public class AICvAnalysisResponse
{
    public List<string> ExtractedSkills { get; set; } = new();
    public int ExperienceYears { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Summary { get; set; } = string.Empty;
}

internal sealed record SingleCvDeepSeekChatRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] SingleCvDeepSeekMessage[] Messages,
    [property: JsonPropertyName("temperature")] double Temperature);

internal sealed record SingleCvDeepSeekMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed class SingleCvDeepSeekCompletion
{
    [JsonPropertyName("choices")]
    public List<SingleCvDeepSeekChoice>? Choices { get; set; }
}

internal sealed class SingleCvDeepSeekChoice
{
    [JsonPropertyName("message")]
    public SingleCvDeepSeekMessageBody? Message { get; set; }
}

internal sealed class SingleCvDeepSeekMessageBody
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

internal sealed class DeepSeekCvResult
{
    [JsonPropertyName("extracted_skills")]
    public List<string>? ExtractedSkills { get; set; }

    [JsonPropertyName("experience_years")]
    public int ExperienceYears { get; set; }

    [JsonPropertyName("education_level")]
    public string? EducationLevel { get; set; }

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
}
