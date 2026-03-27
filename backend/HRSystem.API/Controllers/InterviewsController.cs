using HRSystem.Core.DTOs.Common;
using HRSystem.Core.DTOs.Interview;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR,Interviewer")]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewsController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool upcomingOnly = false)
    {
        var result = await _interviewService.GetAllAsync(upcomingOnly);
        return Ok(ApiResponse<List<InterviewResponseDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _interviewService.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<InterviewResponseDto>.Fail("Interview not found"));
        
        return Ok(ApiResponse<InterviewResponseDto>.Ok(result));
    }

    [HttpGet("application/{applicationId}")]
    public async Task<IActionResult> GetByApplication(int applicationId)
    {
        var result = await _interviewService.GetByApplicationIdAsync(applicationId);
        return Ok(ApiResponse<List<InterviewResponseDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Schedule([FromBody] CreateInterviewDto dto)
    {
        try
        {
            var result = await _interviewService.ScheduleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<InterviewResponseDto>.Ok(result, "Interview scheduled"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<InterviewResponseDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInterviewDto dto)
    {
        var result = await _interviewService.UpdateAsync(id, dto);
        if (result == null) return NotFound(ApiResponse<InterviewResponseDto>.Fail("Interview not found"));
        
        return Ok(ApiResponse<InterviewResponseDto>.Ok(result, "Interview updated"));
    }

    [HttpPut("{id}/result")]
    public async Task<IActionResult> RecordResult(int id, [FromBody] InterviewResultDto dto)
    {
        var result = await _interviewService.RecordResultAsync(id, dto);
        if (result == null) return NotFound(ApiResponse<InterviewResponseDto>.Fail("Interview not found"));

        return Ok(ApiResponse<InterviewResponseDto>.Ok(result, "Result recorded"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _interviewService.CancelAsync(id);
        if (!success) return NotFound(ApiResponse<bool>.Fail("Interview not found"));
        
        return Ok(ApiResponse<bool>.Ok(true, "Interview canceled"));
    }
}
