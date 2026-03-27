using System.ComponentModel.DataAnnotations;

namespace HRSystem.Core.DTOs.Job;

public record CreateJobDto(
    [Required][MaxLength(200)] string Title,
    [Required][MaxLength(100)] string Department,
    [Required] string Description,
    [Required] string Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Location,
    bool IsRemote,
    DateTime? DeadlineAt
);

public record UpdateJobDto(
    [MaxLength(200)] string? Title,
    [MaxLength(100)] string? Department,
    string? Description,
    string? Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Location,
    bool? IsRemote,
    string? Status,
    DateTime? DeadlineAt
);

public record JobResponseDto(
    int Id,
    string Title,
    string Department,
    string Description,
    string Requirements,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Location,
    bool IsRemote,
    string Status,
    DateTime CreatedAt,
    DateTime? DeadlineAt,
    int ApplicationCount
);

public record JobListDto(
    int Id,
    string Title,
    string Department,
    string? Location,
    bool IsRemote,
    string Status,
    decimal? SalaryMin,
    decimal? SalaryMax,
    DateTime CreatedAt,
    DateTime? DeadlineAt,
    int ApplicationCount
);
