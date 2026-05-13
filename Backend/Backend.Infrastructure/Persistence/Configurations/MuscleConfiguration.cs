using Backend.Core.Entities.ExerciseRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class MuscleConfiguration : IEntityTypeConfiguration<Muscle>
{
    public void Configure(EntityTypeBuilder<Muscle> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.ImageUrl).HasMaxLength(500);

        builder.HasMany(m => m.ExerciseMuscles)
               .WithOne(em => em.Muscle)
               .HasForeignKey(em => em.MuscleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
