using HRSystem.Core.DTOs.Common;
using HRSystem.Core.DTOs.Candidate;
using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Core.Interfaces.Services;

namespace HRSystem.Infrastructure.Services;

public class CandidateService : ICandidateService
{
    private readonly IUnitOfWork _unitOfWork;

    public CandidateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CandidateListDto>> GetAllAsync(PaginationParams pagination)
    {
        var rawResult = await _unitOfWork.Candidates.GetAllAsync(pagination);

        var dtoList = rawResult.Items.Select(c => new CandidateListDto(
            c.Id, c.FullName, c.Email, c.TotalScore, c.ExperienceYears, c.EducationLevel,
            c.CreatedAt, c.Skills.Select(s => s.SkillName).Take(5).ToList()
        )).ToList();

        return new PagedResult<CandidateListDto>(dtoList, rawResult.TotalCount, rawResult.Page, rawResult.PageSize, rawResult.TotalPages);
    }

    public async Task<CandidateResponseDto?> GetByIdAsync(int id)
    {
        var c = await _unitOfWork.Candidates.GetByIdWithDetailsAsync(id);
        if (c == null) return null;

        var skills = c.Skills.Select(s => new SkillDto(s.Id, s.SkillName, s.Level?.ToString(), s.Source.ToString())).ToList();

        return new CandidateResponseDto(
            c.Id, c.FullName, c.Email, c.Phone, c.LinkedInUrl, c.PortfolioUrl,
            c.CvFilePath, c.TotalScore, c.ExperienceYears, c.EducationLevel,
            c.AiSummary, c.CreatedAt, skills, c.Applications?.Count ?? 0);
    }

    public Task<PagedResult<CandidateListDto>> SearchAsync(string query, PaginationParams pagination)
    {
        // Simple delegating to GetAllAsync which handles search internally
        var p = new PaginationParams { Page = pagination.Page, PageSize = pagination.PageSize, Search = query, SortDesc = pagination.SortDesc };
        return GetAllAsync(p);
    }

    public async Task<CandidateResponseDto> CreateAsync(CreateCandidateDto dto)
    {
        var existing = await _unitOfWork.Candidates.GetByEmailAsync(dto.Email);
        if (existing != null) throw new Exception("Email already registered");

        var candidate = new Candidate
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            LinkedInUrl = dto.LinkedInUrl,
            PortfolioUrl = dto.PortfolioUrl
        };

        await _unitOfWork.Candidates.CreateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(candidate.Id) ?? throw new Exception("Failed to load candidate");
    }

    public async Task<CandidateResponseDto?> UpdateAsync(int id, UpdateCandidateDto dto)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(id);
        if (candidate == null) return null;

        if (dto.FirstName != null) candidate.FirstName = dto.FirstName;
        if (dto.LastName != null) candidate.LastName = dto.LastName;
        if (dto.Phone != null) candidate.Phone = dto.Phone;
        if (dto.LinkedInUrl != null) candidate.LinkedInUrl = dto.LinkedInUrl;
        if (dto.PortfolioUrl != null) candidate.PortfolioUrl = dto.PortfolioUrl;

        candidate.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _unitOfWork.Candidates.ExistsAsync(id)) return false;
        
        await _unitOfWork.Candidates.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
