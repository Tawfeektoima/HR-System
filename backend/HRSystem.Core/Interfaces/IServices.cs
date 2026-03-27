using HRSystem.Core.DTOs.Job;
using HRSystem.Core.DTOs.Candidate;
using HRSystem.Core.DTOs.Application;
using HRSystem.Core.DTOs.Interview;
using HRSystem.Core.DTOs.Auth;
using HRSystem.Core.DTOs.Analytics;
using HRSystem.Core.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace HRSystem.Core.Interfaces.Services;

public interface IJobService
{
    Task<PagedResult<JobListDto>> GetAllAsync(PaginationParams pagination, string? status = null, string? department = null);
    Task<JobResponseDto?> GetByIdAsync(int id);
    Task<List<JobListDto>> GetOpenJobsAsync();
    Task<JobResponseDto> CreateAsync(CreateJobDto dto, int createdById);
    Task<JobResponseDto?> UpdateAsync(int id, UpdateJobDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ICandidateService
{
    Task<PagedResult<CandidateListDto>> GetAllAsync(PaginationParams pagination);
    Task<CandidateResponseDto?> GetByIdAsync(int id);
    Task<PagedResult<CandidateListDto>> SearchAsync(string query, PaginationParams pagination);
    Task<CandidateResponseDto> CreateAsync(CreateCandidateDto dto);
    Task<CandidateResponseDto?> UpdateAsync(int id, UpdateCandidateDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IApplicationService
{
    Task<PagedResult<ApplicationListDto>> GetAllAsync(ApplicationFilterParams filter);
    Task<ApplicationResponseDto?> GetByIdAsync(int id);
    Task<List<ApplicationListDto>> GetByJobIdAsync(int jobId);
    Task<ApplicationResponseDto> SubmitApplicationAsync(CreateApplicationDto dto, IFormFile cvFile);
    Task<ApplicationResponseDto?> UpdateStatusAsync(int id, UpdateApplicationStatusDto dto, int reviewerId);
    Task<bool> DeleteAsync(int id);
}

public interface IInterviewService
{
    Task<List<InterviewResponseDto>> GetAllAsync(bool upcomingOnly = false);
    Task<InterviewResponseDto?> GetByIdAsync(int id);
    Task<List<InterviewResponseDto>> GetByApplicationIdAsync(int applicationId);
    Task<InterviewResponseDto> ScheduleAsync(CreateInterviewDto dto);
    Task<InterviewResponseDto?> UpdateAsync(int id, UpdateInterviewDto dto);
    Task<InterviewResponseDto?> RecordResultAsync(int id, InterviewResultDto dto);
    Task<bool> CancelAsync(int id);
}

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<ApplicationsPerMonthDto>> GetApplicationsPerMonthAsync(int months = 6);
    Task<List<PipelineFunnelDto>> GetPipelineFunnelAsync();
    Task<List<TopJobDto>> GetTopJobsAsync(int count = 5);
}

public interface IAIService
{
    Task<AICvAnalysisResult> AnalyzeCvAsync(string filePath, int jobId);
}

public record AICvAnalysisResult(
    List<string> ExtractedSkills,
    int ExperienceYears,
    string? EducationLevel,
    decimal Score,
    string Summary
);

public interface IFileStorageService
{
    Task<string> SaveCvFileAsync(IFormFile file, int candidateId);
    Task<byte[]> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}

public interface INotificationService
{
    Task SendToUserAsync(int userId, string title, string message, string type = "Info", int? relatedEntityId = null, string? relatedEntity = null);
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}

public record NotificationDto(
    int Id,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    DateTime CreatedAt
);
