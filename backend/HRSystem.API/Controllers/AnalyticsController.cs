using HRSystem.Core.DTOs.Analytics;
using HRSystem.Core.DTOs.Common;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _analyticsService.GetDashboardStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }

    [HttpGet("pipeline")]
    public async Task<IActionResult> GetPipelineFunnel()
    {
        var data = await _analyticsService.GetPipelineFunnelAsync();
        return Ok(ApiResponse<List<PipelineFunnelDto>>.Ok(data));
    }

    [HttpGet("applications-per-month")]
    public async Task<IActionResult> GetApplicationsPerMonth([FromQuery] int months = 6)
    {
        var data = await _analyticsService.GetApplicationsPerMonthAsync(months);
        return Ok(ApiResponse<List<ApplicationsPerMonthDto>>.Ok(data));
    }

    [HttpGet("top-jobs")]
    public async Task<IActionResult> GetTopJobs([FromQuery] int count = 5)
    {
        var data = await _analyticsService.GetTopJobsAsync(count);
        return Ok(ApiResponse<List<TopJobDto>>.Ok(data));
    }

    [HttpGet("full")]
    public async Task<IActionResult> GetFullAnalytics()
    {
        var stats = await _analyticsService.GetDashboardStatsAsync();
        var monthly = await _analyticsService.GetApplicationsPerMonthAsync(6);
        var pipeline = await _analyticsService.GetPipelineFunnelAsync();
        var jobs = await _analyticsService.GetTopJobsAsync(5);

        var result = new AnalyticsResponseDto(stats, monthly, pipeline, jobs);
        return Ok(ApiResponse<AnalyticsResponseDto>.Ok(result));
    }
}
