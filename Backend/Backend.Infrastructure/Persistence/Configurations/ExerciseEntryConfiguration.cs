using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseEntryConfiguration : IEntityTypeConfiguration<ExerciseEntry>
{
    public void Configure(EntityTypeBuilder<ExerciseEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.MeasureType).IsRequired().HasConversion<string>();

        builder.HasOne(e => e.Exercise)
               .WithMany()
               .HasForeignKey(e => e.ExerciseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ExerciseType)
               .WithMany()
               .HasForeignKey(e => e.ExerciseTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Intervals)
               .WithOne(i => i.ExerciseEntry)
               .HasForeignKey(i => i.ExerciseEntryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
