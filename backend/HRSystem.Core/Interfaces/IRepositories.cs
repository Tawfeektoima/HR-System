using HRSystem.Core.Entities;
using HRSystem.Core.DTOs.Common;

namespace HRSystem.Core.Interfaces;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(int id);
    Task<Job?> GetByIdWithDetailsAsync(int id);
    Task<PagedResult<Job>> GetAllAsync(PaginationParams pagination, string? status = null, string? department = null);
    Task<List<Job>> GetOpenJobsAsync();
    Task<Job> CreateAsync(Job job);
    Task<Job> UpdateAsync(Job job);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetApplicationCountAsync(int jobId);
}

public interface ICandidateRepository
{
    Task<Candidate?> GetByIdAsync(int id);
    Task<Candidate?> GetByIdWithDetailsAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
    Task<PagedResult<Candidate>> GetAllAsync(PaginationParams pagination);
    Task<List<Candidate>> GetTopCandidatesAsync(int count = 10);
    Task<Candidate> CreateAsync(Candidate candidate);
    Task<Candidate> UpdateAsync(Candidate candidate);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
}

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(int id);
    Task<Application?> GetByIdWithDetailsAsync(int id);
    Task<PagedResult<Application>> GetAllAsync(ApplicationFilterParams filter);
    Task<List<Application>> GetByJobIdAsync(int jobId);
    Task<List<Application>> GetByCandidateIdAsync(int candidateId);
    Task<Application?> GetByCandidateAndJobAsync(int candidateId, int jobId);
    Task<Application> CreateAsync(Application application);
    Task<Application> UpdateAsync(Application application);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public interface IInterviewRepository
{
    Task<Interview?> GetByIdAsync(int id);
    Task<Interview?> GetByIdWithDetailsAsync(int id);
    Task<List<Interview>> GetByApplicationIdAsync(int applicationId);
    Task<List<Interview>> GetUpcomingAsync(int? interviewerId = null);
    Task<Interview> CreateAsync(Interview interview);
    Task<Interview> UpdateAsync(Interview interview);
    Task DeleteAsync(int id);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}

public interface IUnitOfWork : IDisposable
{
    IJobRepository Jobs { get; }
    ICandidateRepository Candidates { get; }
    IApplicationRepository Applications { get; }
    IInterviewRepository Interviews { get; }
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync();
}
