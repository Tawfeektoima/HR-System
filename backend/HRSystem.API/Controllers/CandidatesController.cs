using HRSystem.Core.DTOs.Candidate;
using HRSystem.Core.DTOs.Common;
using HRSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _candidateService.GetAllAsync(pagination);
        return Ok(ApiResponse<PagedResult<CandidateListDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _candidateService.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<CandidateResponseDto>.Fail("Candidate not found"));
        
        return Ok(ApiResponse<CandidateResponseDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCandidateDto request)
    {
        var result = await _candidateService.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<CandidateResponseDto>.Fail("Candidate not found"));
        
        return Ok(ApiResponse<CandidateResponseDto>.Ok(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _candidateService.DeleteAsync(id);
        if (!success) return NotFound(ApiResponse<bool>.Fail("Candidate not found"));
        
        return Ok(ApiResponse<bool>.Ok(true, "Candidate deleted"));
    }
}
