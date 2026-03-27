using HRSystem.Core.Entities;
using HRSystem.Core.Interfaces;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HRSystemDbContext _context;
    
    public IJobRepository Jobs { get; private set; }
    public ICandidateRepository Candidates { get; private set; }
    public IApplicationRepository Applications { get; private set; }
    public IInterviewRepository Interviews { get; private set; }
    public IUserRepository Users { get; private set; }

    public UnitOfWork(
        HRSystemDbContext context,
        IJobRepository jobRepository,
        ICandidateRepository candidateRepository,
        IApplicationRepository applicationRepository,
        IInterviewRepository interviewRepository,
        IUserRepository userRepository)
    {
        _context = context;
        Jobs = jobRepository;
        Candidates = candidateRepository;
        Applications = applicationRepository;
        Interviews = interviewRepository;
        Users = userRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
