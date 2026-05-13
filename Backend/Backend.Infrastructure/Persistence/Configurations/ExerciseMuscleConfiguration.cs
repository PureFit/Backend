using Backend.Core.Entities.ExerciseRelated;
using Backend.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseMuscleConfiguration : IEntityTypeConfiguration<ExerciseMuscle>
{
    public void Configure(EntityTypeBuilder<ExerciseMuscle> builder)
    {
        builder.HasKey(em => new { em.ExerciseId, em.MuscleId });

        builder.Property(em => em.Role).IsRequired().HasConversion<string>();
    }
}
