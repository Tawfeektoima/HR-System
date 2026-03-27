using HRSystem.Core.DTOs.Interview;
using HRSystem.Core.Entities;
using HRSystem.Core.Enums;
using HRSystem.Core.Interfaces;
using HRSystem.Core.Interfaces.Services;

namespace HRSystem.Infrastructure.Services;

public class InterviewService : IInterviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public InterviewService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<List<InterviewResponseDto>> GetAllAsync(bool upcomingOnly = false)
    {
        var list = upcomingOnly 
            ? await _unitOfWork.Interviews.GetUpcomingAsync()
            : await _unitOfWork.Interviews.GetUpcomingAsync(); // Modify repo to support past as well if needed
            
        return list.Select(MapToResponse).ToList();
    }

    public async Task<InterviewResponseDto?> GetByIdAsync(int id)
    {
        var interview = await _unitOfWork.Interviews.GetByIdWithDetailsAsync(id);
        return interview == null ? null : MapToResponse(interview);
    }

    public async Task<List<InterviewResponseDto>> GetByApplicationIdAsync(int applicationId)
    {
        var list = await _unitOfWork.Interviews.GetByApplicationIdAsync(applicationId);
        return list.Select(MapToResponse).ToList();
    }

    public async Task<InterviewResponseDto> ScheduleAsync(CreateInterviewDto dto)
    {
        var app = await _unitOfWork.Applications.GetByIdWithDetailsAsync(dto.ApplicationId);
        if (app == null) throw new Exception("Application not found");

        if (!Enum.TryParse<InterviewType>(dto.Type, true, out var type))
            throw new Exception("Invalid interview type");

        var interview = new Interview
        {
            ApplicationId = dto.ApplicationId,
            InterviewerId = dto.InterviewerId,
            Type = type,
            ScheduledAt = dto.ScheduledAt,
            DurationMinutes = dto.DurationMinutes,
            Location = dto.Location,
            IsOnline = dto.IsOnline,
            Result = InterviewResult.Pending
        };

        await _unitOfWork.Interviews.CreateAsync(interview);
        
        // Auto-update Application stage based on Interview Type
        app.Status = type switch
        {
            InterviewType.Phone => ApplicationStatus.PhoneInterview,
            InterviewType.Technical => ApplicationStatus.TechnicalInterview,
            InterviewType.Final => ApplicationStatus.FinalInterview,
            _ => app.Status
        };

        await _unitOfWork.SaveChangesAsync();

        // Notify Interviewer
        if (interview.InterviewerId.HasValue)
        {
            await _notificationService.SendToUserAsync(
                interview.InterviewerId.Value, 
                "New Interview Scheduled", 
                $"You have a {type} interview scheduled with {app.Candidate.FullName} for the {app.Job.Title} role."
            );
        }

        // Generate response
        var result = await _unitOfWork.Interviews.GetByIdWithDetailsAsync(interview.Id);
        return MapToResponse(result!);
    }

    public async Task<InterviewResponseDto?> UpdateAsync(int id, UpdateInterviewDto dto)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(id);
        if (interview == null) return null;

        if (dto.ScheduledAt.HasValue) interview.ScheduledAt = dto.ScheduledAt.Value;
        if (dto.DurationMinutes.HasValue) interview.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.Location != null) interview.Location = dto.Location;
        if (dto.IsOnline.HasValue) interview.IsOnline = dto.IsOnline.Value;
        if (dto.InterviewerId.HasValue) interview.InterviewerId = dto.InterviewerId.Value;

        interview.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Interviews.UpdateAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        var result = await _unitOfWork.Interviews.GetByIdWithDetailsAsync(id);
        return MapToResponse(result!);
    }

    public async Task<InterviewResponseDto?> RecordResultAsync(int id, InterviewResultDto dto)
    {
        var interview = await _unitOfWork.Interviews.GetByIdWithDetailsAsync(id);
        if (interview == null) return null;

        if (Enum.TryParse<InterviewResult>(dto.Result, true, out var resultEnum))
        {
            interview.Result = resultEnum;
        }

        interview.Score = dto.Score;
        if (dto.Notes != null) interview.Notes = dto.Notes;
        interview.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Interviews.UpdateAsync(interview);

        // If interview passed/failed, we could auto-update application here if wanted.
        
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(interview);
    }

    public async Task<bool> CancelAsync(int id)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(id);
        if (interview == null) return false;

        await _unitOfWork.Interviews.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static InterviewResponseDto MapToResponse(Interview i)
    {
        return new InterviewResponseDto(
            i.Id, i.ApplicationId, 
            i.Application?.Candidate?.FullName ?? "Unknown",
            i.Application?.Job?.Title ?? "Unknown",
            i.Interviewer?.FullName,
            i.Type.ToString(), i.ScheduledAt, i.DurationMinutes,
            i.Location, i.IsOnline, i.Result.ToString(), i.Score, i.Notes, i.CreatedAt
        );
    }
}
