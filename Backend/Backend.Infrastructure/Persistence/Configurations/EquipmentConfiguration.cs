using Backend.Core.Entities.ExerciseRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);

        builder.HasMany(e => e.ExerciseEquipments)
               .WithOne(ee => ee.Equipment)
               .HasForeignKey(ee => ee.EquipmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
