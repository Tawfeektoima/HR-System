using System.Data;
using HRSystem.Core.DTOs.Analytics;
using HRSystem.Core.Interfaces.Services;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using HRSystem.Core.Enums;

namespace HRSystem.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly HRSystemDbContext _context;

    public AnalyticsService(HRSystemDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalJobs = await _context.Jobs.CountAsync();
        var openJobs = await _context.Jobs.CountAsync(j => j.Status == Core.Enums.JobStatus.Open);
        var totalCandidates = await _context.Candidates.CountAsync();
        var totalApplications = await _context.Applications.CountAsync();
        var hires = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.Accepted);
        var rejections = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.Rejected);
        var activeInterviews = await _context.Interviews.CountAsync(i => i.ScheduledAt > DateTime.UtcNow);
        
        decimal acceptanceRate = totalApplications > 0 ? (decimal)hires / totalApplications * 100 : 0m;
        decimal avgTime = 0m; 

        return new DashboardStatsDto(
            openJobs,
            totalJobs,
            totalCandidates,
            totalApplications,
            hires,
            rejections,
            activeInterviews,
            Math.Round(acceptanceRate, 2),
            avgTime
        );
    }

    public async Task<List<ApplicationsPerMonthDto>> GetApplicationsPerMonthAsync(int months = 6)
    {
        // Simplified LINQ grouping for SQLite compatibility
        var startDate = DateTime.UtcNow.AddMonths(-months);
        var data = await _context.Applications
            .Where(a => a.AppliedAt >= startDate)
            .GroupBy(a => new { a.AppliedAt.Year, a.AppliedAt.Month })
            .Select(g => new ApplicationsPerMonthDto(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Count(),
                g.Count(x => x.Status == ApplicationStatus.Accepted),
                g.Count(x => x.Status == ApplicationStatus.Rejected)
            ))
            .OrderBy(x => x.MonthLabel)
            .ToListAsync();

        return data;
    }

    public async Task<List<PipelineFunnelDto>> GetPipelineFunnelAsync()
    {
        var data = await _context.Applications
            .GroupBy(a => a.Status)
            .Select(g => new PipelineFunnelDto(
                g.Key.ToString(),
                g.Count(),
                0m 
            ))
            .ToListAsync();

        return data;
    }

    public async Task<List<TopJobDto>> GetTopJobsAsync(int count = 5)
    {
        var data = await _context.Jobs
            .Select(j => new TopJobDto(
                j.Id,
                j.Title,
                j.Department,
                _context.Applications.Count(a => a.JobId == j.Id),
                _context.Applications.Where(a => a.JobId == j.Id).Average(a => (decimal?)a.CvScore) ?? 0m
            ))
            .OrderByDescending(x => x.ApplicationCount)
            .Take(count)
            .ToListAsync();

        return data;
    }
}
