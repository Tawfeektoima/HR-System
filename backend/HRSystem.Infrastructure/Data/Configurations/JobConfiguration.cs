using HRSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(j => j.SalaryMin)
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.SalaryMax)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(j => j.CreatedBy)
            .WithMany(u => u.CreatedJobs)
            .HasForeignKey(j => j.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.Department);
    }
}
