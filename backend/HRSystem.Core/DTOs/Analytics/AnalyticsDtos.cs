namespace HRSystem.Core.DTOs.Analytics;

public record DashboardStatsDto(
    int OpenJobs,
    int TotalJobs,
    int TotalCandidates,
    int TotalApplications,
    int TotalAccepted,
    int TotalRejected,
    int UpcomingInterviews,
    decimal AcceptanceRate,
    decimal AvgDaysToHire
);

public record ApplicationsPerMonthDto(
    string MonthLabel,
    int TotalApplications,
    int Accepted,
    int Rejected
);

public record PipelineFunnelDto(
    string Status,
    int Count,
    decimal Percentage
);

public record TopJobDto(
    int Id,
    string Title,
    string Department,
    int ApplicationCount,
    decimal AvgScore
);

public record AnalyticsResponseDto(
    DashboardStatsDto Stats,
    List<ApplicationsPerMonthDto> ApplicationsPerMonth,
    List<PipelineFunnelDto> Pipeline,
    List<TopJobDto> TopJobs
);
