namespace HRSystem.Core.Entities;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? CvFilePath { get; set; }
    public decimal TotalScore { get; set; } = 0;
    public int ExperienceYears { get; set; } = 0;
    public string? EducationLevel { get; set; }
    public string? AiSummary { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();

    public string FullName => $"{FirstName} {LastName}";
}
