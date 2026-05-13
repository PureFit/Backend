using Backend.Core.Entities.ExerciseRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class BodyPartConfiguration : IEntityTypeConfiguration<BodyPart>
{
    public void Configure(EntityTypeBuilder<BodyPart> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.ImageUrl).HasMaxLength(500);

        builder.HasMany(b => b.Muscles)
               .WithOne(m => m.BodyPart)
               .HasForeignKey(m => m.BodyPartId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.ExerciseBodyParts)
               .WithOne(eb => eb.BodyPart)
               .HasForeignKey(eb => eb.BodyPartId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
