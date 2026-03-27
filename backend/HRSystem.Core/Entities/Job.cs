using HRSystem.Core.Enums;

namespace HRSystem.Core.Entities;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Location { get; set; }
    public bool IsRemote { get; set; } = false;
    public JobStatus Status { get; set; } = JobStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeadlineAt { get; set; }
    public int? CreatedById { get; set; }

    // Navigation
    public User? CreatedBy { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
