using System.ComponentModel.DataAnnotations;

namespace HRSystem.Core.DTOs.Candidate;

public record CreateCandidateDto(
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress] string Email,
    [Phone] string? Phone,
    [Url] string? LinkedInUrl,
    [Url] string? PortfolioUrl
);

public record UpdateCandidateDto(
    [MaxLength(100)] string? FirstName,
    [MaxLength(100)] string? LastName,
    [Phone] string? Phone,
    [Url] string? LinkedInUrl,
    [Url] string? PortfolioUrl
);

public record CandidateResponseDto(
    int Id,
    string FullName,
    string Email,
    string? Phone,
    string? LinkedInUrl,
    string? PortfolioUrl,
    string? CvFilePath,
    decimal TotalScore,
    int ExperienceYears,
    string? EducationLevel,
    string? AiSummary,
    DateTime CreatedAt,
    List<SkillDto> Skills,
    int TotalApplications
);

public record CandidateListDto(
    int Id,
    string FullName,
    string Email,
    decimal TotalScore,
    int ExperienceYears,
    string? EducationLevel,
    DateTime CreatedAt,
    List<string> TopSkills
);

public record SkillDto(
    int Id,
    string SkillName,
    string? Level,
    string Source
);
