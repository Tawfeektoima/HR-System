using System.ComponentModel.DataAnnotations;

namespace HRSystem.Core.DTOs.Interview;

public record CreateInterviewDto(
    [Required] int ApplicationId,
    int? InterviewerId,
    [Required] string Type,
    [Required] DateTime ScheduledAt,
    int DurationMinutes = 60,
    string? Location = null,
    bool IsOnline = true
);

public record UpdateInterviewDto(
    DateTime? ScheduledAt,
    int? DurationMinutes,
    string? Location,
    bool? IsOnline,
    int? InterviewerId
);

public record InterviewResultDto(
    [Required] string Result,
    decimal? Score,
    string? Notes
);

public record InterviewResponseDto(
    int Id,
    int ApplicationId,
    string CandidateName,
    string JobTitle,
    string? InterviewerName,
    string Type,
    DateTime ScheduledAt,
    int DurationMinutes,
    string? Location,
    bool IsOnline,
    string Result,
    decimal? Score,
    string? Notes,
    DateTime CreatedAt
);
