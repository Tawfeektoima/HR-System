using HRSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Data.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SkillName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Level)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Source)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(HRSystem.Core.Enums.SkillSource.AI);

        builder.HasIndex(s => s.SkillName);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(HRSystem.Core.Enums.NotificationType.Info);

        builder.Property(n => n.RelatedEntity)
            .HasMaxLength(50);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => n.IsRead);
    }
}
