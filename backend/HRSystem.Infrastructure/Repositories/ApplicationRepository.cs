using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Core.DTOs.Common;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HRSystem.Core.Enums;

namespace HRSystem.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly HRSystemDbContext _context;

    public ApplicationRepository(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Application?> GetByIdAsync(int id)
    {
        return await _context.Applications.FindAsync(id);
    }

    public async Task<Application?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Applications
            .Include(a => a.Candidate)
                .ThenInclude(c => c.Skills)
            .Include(a => a.Job)
            .Include(a => a.Interviews)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<PagedResult<Application>> GetAllAsync(ApplicationFilterParams filter)
    {
        var query = _context.Applications
            .Include(a => a.Candidate)
            .Include(a => a.Job)
            .AsQueryable();

        if (filter.JobId.HasValue)
            query = query.Where(a => a.JobId == filter.JobId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<ApplicationStatus>(filter.Status, true, out var parsedStatus))
            query = query.Where(a => a.Status == parsedStatus);

        if (filter.FromDate.HasValue)
            query = query.Where(a => a.AppliedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(a => a.AppliedAt <= filter.ToDate.Value);

        if (filter.MinScore.HasValue)
            query = query.Where(a => a.CvScore >= filter.MinScore.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(a => a.Candidate.FirstName.Contains(filter.Search) ||
                                     a.Candidate.LastName.Contains(filter.Search) ||
                                     a.Candidate.Email.Contains(filter.Search));
        }

        query = filter.SortDesc ? query.OrderByDescending(a => a.AppliedAt) : query.OrderBy(a => a.AppliedAt);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize)
                               .Take(filter.PageSize)
                               .ToListAsync();

        return new PagedResult<Application>(items, totalCount, filter.Page, filter.PageSize, (int)Math.Ceiling(totalCount / (double)filter.PageSize));
    }

    public async Task<List<Application>> GetByJobIdAsync(int jobId)
    {
        return await _context.Applications
            .Include(a => a.Candidate)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.CvScore)
            .ToListAsync();
    }

    public async Task<List<Application>> GetByCandidateIdAsync(int candidateId)
    {
        return await _context.Applications
            .Include(a => a.Job)
            .Where(a => a.CandidateId == candidateId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task<Application?> GetByCandidateAndJobAsync(int candidateId, int jobId)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.CandidateId == candidateId && a.JobId == jobId);
    }

    public async Task<Application> CreateAsync(Application application)
    {
        _context.Applications.Add(application);
        return await Task.FromResult(application);
    }

    public async Task<Application> UpdateAsync(Application application)
    {
        _context.Applications.Update(application);
        return await Task.FromResult(application);
    }

    public async Task DeleteAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application != null)
        {
            _context.Applications.Remove(application);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Applications.AnyAsync(a => a.Id == id);
    }
}
