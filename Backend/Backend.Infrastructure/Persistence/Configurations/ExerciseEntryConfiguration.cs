using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseEntryConfiguration : IEntityTypeConfiguration<ExerciseEntry>
{
    public void Configure(EntityTypeBuilder<ExerciseEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExerciseId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.MeasureType).IsRequired().HasConversion<string>();
        builder.Property(e => e.Parameters).HasColumnType("jsonb");
        builder.Property(e => e.TargetMuscles).HasColumnType("jsonb");
        builder.Property(e => e.SecondaryMuscles).HasColumnType("jsonb");
        builder.Property(e => e.BodyParts).HasColumnType("jsonb");

        builder.HasMany(e => e.Intervals)
               .WithOne(i => i.ExerciseEntry)
               .HasForeignKey(i => i.ExerciseEntryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
