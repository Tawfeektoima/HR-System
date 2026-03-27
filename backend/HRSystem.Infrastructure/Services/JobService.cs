using HRSystem.Core.DTOs.Common;
using HRSystem.Core.DTOs.Job;
using HRSystem.Core.Entities;
using HRSystem.Core.Enums;
using HRSystem.Core.Interfaces;
using HRSystem.Core.Interfaces.Services;

namespace HRSystem.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<JobListDto>> GetAllAsync(PaginationParams pagination, string? status = null, string? department = null)
    {
        var rawResult = await _unitOfWork.Jobs.GetAllAsync(pagination, status, department);
        
        var dtoList = rawResult.Items.Select(j => new JobListDto(
            j.Id, j.Title, j.Department, j.Location, j.IsRemote, 
            j.Status.ToString(), j.SalaryMin, j.SalaryMax, 
            j.CreatedAt, j.DeadlineAt, j.Applications?.Count ?? 0)).ToList();

        return new PagedResult<JobListDto>(dtoList, rawResult.TotalCount, rawResult.Page, rawResult.PageSize, rawResult.TotalPages);
    }

    public async Task<JobResponseDto?> GetByIdAsync(int id)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(id);
        if (job == null) return null;

        var count = await _unitOfWork.Jobs.GetApplicationCountAsync(id);

        return new JobResponseDto(
            job.Id, job.Title, job.Department, job.Description, job.Requirements,
            job.SalaryMin, job.SalaryMax, job.Location, job.IsRemote,
            job.Status.ToString(), job.CreatedAt, job.DeadlineAt, count);
    }

    public async Task<List<JobListDto>> GetOpenJobsAsync()
    {
        var jobs = await _unitOfWork.Jobs.GetOpenJobsAsync();
        return jobs.Select(j => new JobListDto(
            j.Id, j.Title, j.Department, j.Location, j.IsRemote, 
            j.Status.ToString(), j.SalaryMin, j.SalaryMax, 
            j.CreatedAt, j.DeadlineAt, 0)).ToList();
    }

    public async Task<JobResponseDto> CreateAsync(CreateJobDto dto, int createdById)
    {
        var job = new Job
        {
            Title = dto.Title,
            Department = dto.Department,
            Description = dto.Description,
            Requirements = dto.Requirements,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            Location = dto.Location,
            IsRemote = dto.IsRemote,
            DeadlineAt = dto.DeadlineAt,
            CreatedById = createdById,
            Status = JobStatus.Open
        };

        await _unitOfWork.Jobs.CreateAsync(job);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(job.Id) ?? throw new Exception("Failed to retrieve created job");
    }

    public async Task<JobResponseDto?> UpdateAsync(int id, UpdateJobDto dto)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(id);
        if (job == null) return null;

        if (dto.Title != null) job.Title = dto.Title;
        if (dto.Department != null) job.Department = dto.Department;
        if (dto.Description != null) job.Description = dto.Description;
        if (dto.Requirements != null) job.Requirements = dto.Requirements;
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax;
        if (dto.Location != null) job.Location = dto.Location;
        if (dto.IsRemote.HasValue) job.IsRemote = dto.IsRemote.Value;
        if (dto.DeadlineAt.HasValue) job.DeadlineAt = dto.DeadlineAt;

        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<JobStatus>(dto.Status, true, out var parsedStatus))
        {
            job.Status = parsedStatus;
        }

        job.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Jobs.UpdateAsync(job);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(job.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _unitOfWork.Jobs.ExistsAsync(id)) return false;

        await _unitOfWork.Jobs.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
