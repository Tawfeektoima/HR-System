namespace HRSystem.Core.DTOs.AI;

public record CvComparisonScoreItemDto(
    int ApplicationId,
    string CandidateName,
    decimal Score,
    string? Reason
);

public record CvComparisonResponseDto(
    int JobId,
    string JobTitle,
    string OverallSummary,
    IReadOnlyList<CvComparisonScoreItemDto> Scores
);
