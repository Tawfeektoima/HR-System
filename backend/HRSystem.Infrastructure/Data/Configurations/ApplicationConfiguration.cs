using HRSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Data.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.CvScore)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.HasOne(a => a.Candidate)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.ReviewedBy)
            .WithMany()
            .HasForeignKey(a => a.ReviewedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.JobId);
        
        // UNIQUE (CandidateId, JobId)
        builder.HasIndex(a => new { a.CandidateId, a.JobId })
            .IsUnique();
    }
}
