namespace HRSystem.Core.Enums;

public enum JobStatus
{
    Draft = 0,
    Open = 1,
    Closed = 2
}

public enum UserRole
{
    Admin = 1,
    HR = 2,
    Interviewer = 3
}

public enum SkillLevel
{
    Beginner = 1,
    Intermediate = 2,
    Senior = 3,
    Expert = 4
}

public enum SkillSource
{
    AI = 1,
    Manual = 2
}

public enum InterviewType
{
    Phone = 1,
    Technical = 2,
    HR = 3,
    Final = 4
}

public enum InterviewResult
{
    Pending = 0,
    Passed = 1,
    Failed = 2,
    NoShow = 3
}

public enum NotificationType
{
    Info = 1,
    Success = 2,
    Warning = 3,
    Error = 4
}
