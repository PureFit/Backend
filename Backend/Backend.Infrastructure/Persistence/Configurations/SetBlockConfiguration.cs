using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class SetBlockConfiguration : IEntityTypeConfiguration<SetBlock>
{
    public void Configure(EntityTypeBuilder<SetBlock> builder)
    {
        builder.HasKey(sb => sb.Id);

        builder.Property(sb => sb.SetsCount).IsRequired();
        builder.Property(sb => sb.RestBetweenSetsSeconds).IsRequired();
        builder.Property(sb => sb.RestAfterBlockSeconds).IsRequired();

        builder.HasMany(sb => sb.ExerciseEntries)
               .WithOne(e => e.SetBlock)
               .HasForeignKey(e => e.SetBlockId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
