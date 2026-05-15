using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class AIPlanConfiguration : IEntityTypeConfiguration<AIPlan>
{
    public void Configure(EntityTypeBuilder<AIPlan> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        builder.Property(p => p.PlanType).IsRequired().HasConversion<string>();
        builder.Property(p => p.PlanSubType).IsRequired().HasConversion<string>();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.GoalMetadata).HasColumnType("jsonb");
        builder.Property(p => p.AvailableEquipment).HasColumnType("jsonb");
        builder.Property(p => p.SessionsPerWeek).IsRequired();
        builder.Property(p => p.SessionDurationMinutes).IsRequired();
        builder.Property(p => p.PlanDurationWeeks).IsRequired();

        // Основная связь: WeekPlan принадлежит AIPlan (cascade delete).
        builder.HasMany(p => p.WeekPlans)
               .WithOne(w => w.AIPlan)
               .HasForeignKey(w => w.AIPlanId)
               .OnDelete(DeleteBehavior.Cascade);

        // CurrentWeek — опциональная ссылка на одну из WeekPlan.
        // NoAction чтобы избежать цикличного cascade delete.
        builder.HasOne(p => p.CurrentWeek)
               .WithMany()
               .HasForeignKey(p => p.CurrentWeekId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
