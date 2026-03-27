using HRSystem.Core.Enums;

namespace HRSystem.Core.Entities;

public class Application
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public decimal CvScore { get; set; } = 0;
    public string? HrNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? ReviewedById { get; set; }

    // Navigation
    public Candidate Candidate { get; set; } = null!;
    public Job Job { get; set; } = null!;
    public User? ReviewedBy { get; set; }
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
