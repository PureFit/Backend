using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseIntervalConfiguration : IEntityTypeConfiguration<ExerciseInterval>
{
    public void Configure(EntityTypeBuilder<ExerciseInterval> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.DurationSeconds).IsRequired();
        builder.Property(i => i.Parameters).HasColumnType("jsonb");
    }
}
