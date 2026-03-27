using HRSystem.Core.Enums;

namespace HRSystem.Core.Entities;

public class Interview
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int? InterviewerId { get; set; }
    public InterviewType Type { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? Location { get; set; }
    public bool IsOnline { get; set; } = true;
    public InterviewResult Result { get; set; } = InterviewResult.Pending;
    public decimal? Score { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Application Application { get; set; } = null!;
    public User? Interviewer { get; set; }
}
