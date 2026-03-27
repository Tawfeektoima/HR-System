using HRSystem.Core.DTOs.Common;
using HRSystem.Core.DTOs.Job;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, [FromQuery] string? status, [FromQuery] string? department)
    {
        var result = await _jobService.GetAllAsync(pagination, status, department);
        return Ok(ApiResponse<PagedResult<JobListDto>>.Ok(result));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOpenJobs()
    {
        var result = await _jobService.GetOpenJobsAsync();
        return Ok(ApiResponse<List<JobListDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _jobService.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<JobResponseDto>.Fail("Job not found"));
        
        return Ok(ApiResponse<JobResponseDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] CreateJobDto request)
    {
        // Get user ID from claims
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();

        var result = await _jobService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<JobResponseDto>.Ok(result, "Job created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJobDto request)
    {
        var result = await _jobService.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<JobResponseDto>.Fail("Job not found"));
        
        return Ok(ApiResponse<JobResponseDto>.Ok(result, "Job updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _jobService.DeleteAsync(id);
        if (!success) return NotFound(ApiResponse<bool>.Fail("Job not found"));
        
        return Ok(ApiResponse<bool>.Ok(true, "Job deleted successfully"));
    }
}
