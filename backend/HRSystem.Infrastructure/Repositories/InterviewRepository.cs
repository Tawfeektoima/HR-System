using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly HRSystemDbContext _context;

    public InterviewRepository(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Interview?> GetByIdAsync(int id)
    {
        return await _context.Interviews.FindAsync(id);
    }

    public async Task<Interview?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Candidate)
            .Include(i => i.Application)
                .ThenInclude(a => a.Job)
            .Include(i => i.Interviewer)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Interview>> GetByApplicationIdAsync(int applicationId)
    {
        return await _context.Interviews
            .Include(i => i.Interviewer)
            .Where(i => i.ApplicationId == applicationId)
            .OrderBy(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<List<Interview>> GetUpcomingAsync(int? interviewerId = null)
    {
        var query = _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Candidate)
            .Include(i => i.Application)
                .ThenInclude(a => a.Job)
            .Where(i => i.ScheduledAt >= DateTime.UtcNow);

        if (interviewerId.HasValue)
        {
            query = query.Where(i => i.InterviewerId == interviewerId.Value);
        }

        return await query.OrderBy(i => i.ScheduledAt).ToListAsync();
    }

    public async Task<Interview> CreateAsync(Interview interview)
    {
        _context.Interviews.Add(interview);
        return await Task.FromResult(interview);
    }

    public async Task<Interview> UpdateAsync(Interview interview)
    {
        _context.Interviews.Update(interview);
        return await Task.FromResult(interview);
    }

    public async Task DeleteAsync(int id)
    {
        var interview = await _context.Interviews.FindAsync(id);
        if (interview != null)
        {
            _context.Interviews.Remove(interview);
        }
    }
}
