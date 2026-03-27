using HRSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Data.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.Result)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.Score)
            .HasColumnType("decimal(5,2)");

        builder.Property(i => i.Location)
            .HasMaxLength(500);

        builder.HasOne(i => i.Application)
            .WithMany(a => a.Interviews)
            .HasForeignKey(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Interviewer)
            .WithMany(u => u.Interviews)
            .HasForeignKey(i => i.InterviewerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.ScheduledAt);
    }
}
