using HRSystem.Core.Enums;

namespace HRSystem.Core.Entities;

public class Skill
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public SkillLevel? Level { get; set; }
    public SkillSource Source { get; set; } = SkillSource.AI;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Candidate Candidate { get; set; } = null!;
}

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}
