using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class PlanTrainingConfiguration : IEntityTypeConfiguration<PlanTraining>
{
    public void Configure(EntityTypeBuilder<PlanTraining> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Description).IsRequired().HasMaxLength(500);
        builder.Property(pt => pt.StartPlannedDate).IsRequired();
        builder.Property(pt => pt.EndPlannedDate).IsRequired();

        // TrainingSet — шаблон тренировки, принадлежит этой PlanTraining.
        builder.HasOne(pt => pt.TrainingSet)
               .WithOne(ts => ts.PlanTraining)
               .HasForeignKey<TrainingSet>(ts => ts.PlanTrainingId)
               .OnDelete(DeleteBehavior.Cascade);

        // TrainingSession настроена в TrainingSessionConfiguration (1:1, NoAction).
    }
}
