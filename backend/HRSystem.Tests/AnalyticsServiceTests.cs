using HRSystem.Core.Entities;
using HRSystem.Core.Enums;
using HRSystem.Infrastructure.Data;
using HRSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Tests;

public class AnalyticsServiceTests
{
    private static HRSystemDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HRSystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new HRSystemDbContext(options);
    }

    [Fact]
    public async Task GetPipelineFunnelAsync_EmptyDatabase_ReturnsEmptyList()
    {
        await using var ctx = CreateContext();
        var sut = new AnalyticsService(ctx);

        var result = await sut.GetPipelineFunnelAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPipelineFunnelAsync_PercentagesSumTo100()
    {
        await using var ctx = CreateContext();
        var job = new Job
        {
            Title = "Test",
            Department = "Eng",
            Description = "Desc",
            Requirements = "Req",
            Status = JobStatus.Open,
        };
        ctx.Jobs.Add(job);
        await ctx.SaveChangesAsync();

        var c1 = new Candidate { FirstName = "A", LastName = "One", Email = "a1@test.com" };
        var c2 = new Candidate { FirstName = "B", LastName = "Two", Email = "b2@test.com" };
        var c3 = new Candidate { FirstName = "C", LastName = "Thr", Email = "c3@test.com" };
        ctx.Candidates.AddRange(c1, c2, c3);
        await ctx.SaveChangesAsync();

        ctx.Applications.Add(new Application
        {
            JobId = job.Id,
            CandidateId = c1.Id,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
        });
        ctx.Applications.Add(new Application
        {
            JobId = job.Id,
            CandidateId = c2.Id,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
        });
        ctx.Applications.Add(new Application
        {
            JobId = job.Id,
            CandidateId = c3.Id,
            Status = ApplicationStatus.Rejected,
            AppliedAt = DateTime.UtcNow,
        });
        await ctx.SaveChangesAsync();

        var sut = new AnalyticsService(ctx);
        var result = await sut.GetPipelineFunnelAsync();

        Assert.Equal(2, result.Count);
        var sumPct = result.Sum(x => x.Percentage);
        Assert.Equal(100m, sumPct);
        var applied = result.Single(x => x.Status == nameof(ApplicationStatus.Applied));
        var rejected = result.Single(x => x.Status == nameof(ApplicationStatus.Rejected));
        Assert.Equal(2, applied.Count);
        Assert.Equal(1, rejected.Count);
        Assert.Equal(66.67m, applied.Percentage, 2);
        Assert.Equal(33.33m, rejected.Percentage, 2);
    }
}
