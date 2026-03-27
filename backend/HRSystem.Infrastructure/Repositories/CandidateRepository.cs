using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Core.DTOs.Common;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly HRSystemDbContext _context;

    public CandidateRepository(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        return await _context.Candidates.FindAsync(id);
    }

    public async Task<Candidate?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Candidates
            .Include(c => c.Skills)
            .Include(c => c.Applications)
                .ThenInclude(a => a.Job)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<PagedResult<Candidate>> GetAllAsync(PaginationParams pagination)
    {
        var query = _context.Candidates.Include(c => c.Skills).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Search))
        {
            query = query.Where(c => c.FirstName.Contains(pagination.Search) ||
                                     c.LastName.Contains(pagination.Search) ||
                                     c.Email.Contains(pagination.Search) ||
                                     c.Skills.Any(s => s.SkillName.Contains(pagination.Search)));
        }

        query = pagination.SortDesc 
            ? query.OrderByDescending(c => c.TotalScore) 
            : query.OrderBy(c => c.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pagination.Page - 1) * pagination.PageSize)
                               .Take(pagination.PageSize)
                               .ToListAsync();
                               
        return new PagedResult<Candidate>(items, totalCount, pagination.Page, pagination.PageSize, (int)Math.Ceiling(totalCount / (double)pagination.PageSize));
    }

    public async Task<List<Candidate>> GetTopCandidatesAsync(int count = 10)
    {
        return await _context.Candidates
            .Include(c => c.Skills)
            .OrderByDescending(c => c.TotalScore)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Candidate> CreateAsync(Candidate candidate)
    {
        _context.Candidates.Add(candidate);
        return await Task.FromResult(candidate);
    }

    public async Task<Candidate> UpdateAsync(Candidate candidate)
    {
        _context.Candidates.Update(candidate);
        return await Task.FromResult(candidate);
    }

    public async Task DeleteAsync(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate != null)
        {
            _context.Candidates.Remove(candidate);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Candidates.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Candidates.AnyAsync(c => c.Email == email);
    }
}
