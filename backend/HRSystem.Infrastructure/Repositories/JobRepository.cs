using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Core.DTOs.Common;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly HRSystemDbContext _context;

    public JobRepository(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        return await _context.Jobs.FindAsync(id);
    }

    public async Task<Job?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Jobs
            .Include(j => j.CreatedBy)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<PagedResult<Job>> GetAllAsync(PaginationParams pagination, string? status = null, string? department = null)
    {
        var query = _context.Jobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<HRSystem.Core.Enums.JobStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(j => j.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(j => j.Department.Contains(department));
        }

        if (!string.IsNullOrWhiteSpace(pagination.Search))
        {
            query = query.Where(j => j.Title.Contains(pagination.Search) || j.Description.Contains(pagination.Search));
        }

        if (pagination.SortDesc)
            query = query.OrderByDescending(j => j.CreatedAt);
        else
            query = query.OrderBy(j => j.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pagination.Page - 1) * pagination.PageSize)
                               .Take(pagination.PageSize)
                               .ToListAsync();

        return new PagedResult<Job>(
            Items: items,
            TotalCount: totalCount,
            Page: pagination.Page,
            PageSize: pagination.PageSize,
            TotalPages: (int)Math.Ceiling(totalCount / (double)pagination.PageSize));
    }

    public async Task<List<Job>> GetOpenJobsAsync()
    {
        return await _context.Jobs
            .Where(j => j.Status == HRSystem.Core.Enums.JobStatus.Open && (j.DeadlineAt == null || j.DeadlineAt > DateTime.UtcNow))
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<Job> CreateAsync(Job job)
    {
        _context.Jobs.Add(job);
        return await Task.FromResult(job);
    }

    public async Task<Job> UpdateAsync(Job job)
    {
        _context.Jobs.Update(job);
        return await Task.FromResult(job);
    }

    public async Task DeleteAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job != null)
        {
            _context.Jobs.Remove(job);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Jobs.AnyAsync(j => j.Id == id);
    }

    public async Task<int> GetApplicationCountAsync(int jobId)
    {
        return await _context.Applications.CountAsync(a => a.JobId == jobId);
    }
}
