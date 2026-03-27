using HRSystem.Core.DTOs.Application;
using HRSystem.Core.DTOs.Common;
using HRSystem.Core.Entities;
using HRSystem.Core.Enums;
using HRSystem.Core.Interfaces;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace HRSystem.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;
    private readonly IFileStorageService _fileStorageService;

    public ApplicationService(IUnitOfWork unitOfWork, IAIService aiService, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<ApplicationListDto>> GetAllAsync(ApplicationFilterParams filter)
    {
        var rawResult = await _unitOfWork.Applications.GetAllAsync(filter);
        var dtoItems = rawResult.Items.Select(a => new ApplicationListDto(
            a.Id, a.Candidate.FullName, a.Candidate.Email, a.Job.Title, 
            a.Status.ToString(), a.CvScore, a.AppliedAt
        )).ToList();

        return new PagedResult<ApplicationListDto>(dtoItems, rawResult.TotalCount, rawResult.Page, rawResult.PageSize, rawResult.TotalPages);
    }

    public async Task<ApplicationResponseDto?> GetByIdAsync(int id)
    {
        var a = await _unitOfWork.Applications.GetByIdWithDetailsAsync(id);
        if (a == null) return null;

        return new ApplicationResponseDto(
            a.Id, a.CandidateId, a.Candidate.FullName, a.Candidate.Email,
            a.JobId, a.Job.Title, a.Job.Department, a.Status.ToString(),
            a.CvScore, a.HrNotes, a.RejectionReason, a.AppliedAt, a.UpdatedAt,
            a.Candidate.Skills.Select(s => s.SkillName).ToList()
        );
    }

    public async Task<List<ApplicationListDto>> GetByJobIdAsync(int jobId)
    {
        var list = await _unitOfWork.Applications.GetByJobIdAsync(jobId);
        return list.Select(a => new ApplicationListDto(
            a.Id, a.Candidate.FullName, a.Candidate.Email, a.Job.Title, 
            a.Status.ToString(), a.CvScore, a.AppliedAt
        )).ToList();
    }

    public async Task<ApplicationResponseDto> SubmitApplicationAsync(CreateApplicationDto dto, IFormFile cvFile)
    {
        // 1. Ensure Job Exists
        var job = await _unitOfWork.Jobs.GetByIdAsync(dto.JobId);
        if (job == null) throw new Exception("Job not found");

        // 2. Find or Create Candidate
        var candidate = await _unitOfWork.Candidates.GetByEmailAsync(dto.Email);
        if (candidate == null)
        {
            candidate = new Candidate 
            { 
                FirstName = dto.FirstName, 
                LastName = dto.LastName, 
                Email = dto.Email,
                Phone = dto.Phone,
                LinkedInUrl = dto.LinkedInUrl
            };
            await _unitOfWork.Candidates.CreateAsync(candidate);
            await _unitOfWork.SaveChangesAsync();
        }

        // 3. Save CV File
        var cvPath = await _fileStorageService.SaveCvFileAsync(cvFile, candidate.Id);
        candidate.CvFilePath = cvPath;

        // 4. Create Application
        var existingApp = await _unitOfWork.Applications.GetByCandidateAndJobAsync(candidate.Id, dto.JobId);
        if (existingApp != null) throw new Exception("Candidate already applied for this job");

        var app = new Application
        {
            CandidateId = candidate.Id,
            JobId = dto.JobId,
            Status = ApplicationStatus.Applied
        };
        await _unitOfWork.Applications.CreateAsync(app);
        await _unitOfWork.SaveChangesAsync();

        // 5. Call AI Service to parse CV & Calculate Score
        // Get full local path to send to Python service
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cvPath.TrimStart('/'));
        
        try
        {
            var aiResult = await _aiService.AnalyzeCvAsync(fullPath, dto.JobId, job.Requirements);
            
            app.CvScore = aiResult.Score;
            candidate.AiSummary = aiResult.Summary;
            candidate.TotalScore = aiResult.Score; // Simplified logic
            candidate.EducationLevel = aiResult.EducationLevel;
            candidate.ExperienceYears = aiResult.ExperienceYears;

            foreach(var skill in aiResult.ExtractedSkills)
            {
                if (!candidate.Skills.Any(s => s.SkillName.ToLower() == skill.ToLower()))
                {
                    candidate.Skills.Add(new Skill { SkillName = skill, Source = SkillSource.AI });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error, but application was already saved
            app.HrNotes = $"AI Analysis failed: {ex.Message}";
            await _unitOfWork.SaveChangesAsync();
        }

        return await GetByIdAsync(app.Id) ?? throw new Exception("Failed to return application data");
    }

    public async Task<ApplicationResponseDto?> UpdateStatusAsync(int id, UpdateApplicationStatusDto dto, int reviewerId)
    {
        var app = await _unitOfWork.Applications.GetByIdAsync(id);
        if (app == null) return null;

        if (Enum.TryParse<ApplicationStatus>(dto.Status, true, out var newStatus))
        {
            app.Status = newStatus;
        }

        if (dto.HrNotes != null) app.HrNotes = dto.HrNotes;
        if (dto.RejectionReason != null) app.RejectionReason = dto.RejectionReason;

        app.ReviewedById = reviewerId;
        app.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Applications.UpdateAsync(app);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _unitOfWork.Applications.ExistsAsync(id)) return false;
        
        await _unitOfWork.Applications.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
