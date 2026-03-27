using HRSystem.Core.DTOs.Application;
using HRSystem.Core.DTOs.Common;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR,Interviewer")]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationFilterParams filter)
    {
        var result = await _applicationService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<ApplicationListDto>>.Ok(result));
    }

    [HttpGet("job/{jobId}")]
    [Authorize(Roles = "Admin,HR,Interviewer")]
    public async Task<IActionResult> GetByJobId(int jobId)
    {
        var result = await _applicationService.GetByJobIdAsync(jobId);
        return Ok(ApiResponse<List<ApplicationListDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,HR,Interviewer")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _applicationService.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<ApplicationResponseDto>.Fail("Application not found"));
        
        return Ok(ApiResponse<ApplicationResponseDto>.Ok(result));
    }

    // This is the public endpoint for candidates to apply
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Submit([FromForm] CreateApplicationDto dto, IFormFile cvFile)
    {
        if (cvFile == null || cvFile.Length == 0)
            return BadRequest(ApiResponse<ApplicationResponseDto>.Fail("CV file is required"));

        try
        {
            var result = await _applicationService.SubmitApplicationAsync(dto, cvFile);
            return Ok(ApiResponse<ApplicationResponseDto>.Ok(result, "Application submitted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApplicationResponseDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationStatusDto dto)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out var userId)) return Unauthorized();

        var result = await _applicationService.UpdateStatusAsync(id, dto, userId);
        if (result == null) return NotFound(ApiResponse<ApplicationResponseDto>.Fail("Application not found"));

        return Ok(ApiResponse<ApplicationResponseDto>.Ok(result, "Status updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _applicationService.DeleteAsync(id);
        if (!success) return NotFound(ApiResponse<bool>.Fail("Application not found"));
        
        return Ok(ApiResponse<bool>.Ok(true, "Application deleted"));
    }
}
