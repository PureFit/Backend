using Backend.Core.Entities.ExerciseRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.GifUrl).HasMaxLength(500);
        builder.Property(e => e.Overview).HasMaxLength(2000);
        builder.Property(e => e.Category).IsRequired().HasConversion<string>();
        builder.Property(e => e.Instructions).HasColumnType("jsonb");
        builder.Property(e => e.Tips).HasColumnType("jsonb");
        builder.Property(e => e.Variations).HasColumnType("jsonb");
        builder.Property(e => e.Keywords).HasColumnType("jsonb");

        builder.HasMany(e => e.ExerciseTypes)
               .WithOne(t => t.Exercise)
               .HasForeignKey(t => t.ExerciseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ExerciseMuscles)
               .WithOne(em => em.Exercise)
               .HasForeignKey(em => em.ExerciseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ExerciseEquipments)
               .WithOne(ee => ee.Exercise)
               .HasForeignKey(ee => ee.ExerciseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ExerciseBodyParts)
               .WithOne(eb => eb.Exercise)
               .HasForeignKey(eb => eb.ExerciseId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
