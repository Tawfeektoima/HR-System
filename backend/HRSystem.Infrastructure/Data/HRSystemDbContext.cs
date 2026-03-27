using HRSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Data;

public class HRSystemDbContext : DbContext
{
    public HRSystemDbContext(DbContextOptions<HRSystemDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Candidate> Candidates { get; set; } = null!;
    public DbSet<Application> Applications { get; set; } = null!;
    public DbSet<Interview> Interviews { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HRSystemDbContext).Assembly);
    }
}
