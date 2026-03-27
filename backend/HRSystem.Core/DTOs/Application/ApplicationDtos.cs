using System.ComponentModel.DataAnnotations;

namespace HRSystem.Core.DTOs.Application;

public record CreateApplicationDto(
    [Required] int JobId,
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress] string Email,
    [Phone] string? Phone,
    [Url] string? LinkedInUrl
    // CV file is sent as IFormFile separately
);

public record UpdateApplicationStatusDto(
    [Required] string Status,
    string? HrNotes,
    string? RejectionReason
);

public record ApplicationResponseDto(
    int Id,
    int CandidateId,
    string CandidateName,
    string CandidateEmail,
    int JobId,
    string JobTitle,
    string JobDepartment,
    string Status,
    decimal CvScore,
    string? HrNotes,
    string? RejectionReason,
    DateTime AppliedAt,
    DateTime UpdatedAt,
    List<string> CandidateSkills
);

public record ApplicationListDto(
    int Id,
    string CandidateName,
    string CandidateEmail,
    string JobTitle,
    string Status,
    decimal CvScore,
    DateTime AppliedAt
);
