using HRSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace HRSystem.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AICvAnalysisResult> AnalyzeCvAsync(string filePath, int jobId)
    {
        var aiBaseUrl = _configuration["AIServiceUrl"] ?? "http://localhost:8000";
        var fileInfo = new FileInfo(filePath);
        
        using var multipartFormContent = new MultipartFormDataContent();
        
        var fileStreamContent = new StreamContent(File.OpenRead(filePath));
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileInfo.Name);
        multipartFormContent.Add(new StringContent(jobId.ToString()), name: "job_id");

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
}

public class AICvAnalysisResponse
{
    public List<string> ExtractedSkills { get; set; } = new();
    public int ExperienceYears { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Summary { get; set; } = string.Empty;
}
