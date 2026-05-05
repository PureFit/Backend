using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class WeekPlanConfiguration : IEntityTypeConfiguration<WeekPlan>
{
    public void Configure(EntityTypeBuilder<WeekPlan> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Description).IsRequired().HasMaxLength(500);
        builder.Property(w => w.WeekStatus).IsRequired().HasConversion<string>();
        builder.Property(w => w.StartDate).IsRequired();
        builder.Property(w => w.EndDate).IsRequired();

        builder.HasMany(w => w.PlanTrainings)
               .WithOne(pt => pt.WeekPlan)
               .HasForeignKey(pt => pt.WeekPlanId)
               .OnDelete(DeleteBehavior.Cascade);

        // InternalTrainings и ExternalTrainings настроены в их конфигах.
    }
}
