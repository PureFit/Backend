using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class TrainingSetConfiguration : IEntityTypeConfiguration<TrainingSet>
{
    public void Configure(EntityTypeBuilder<TrainingSet> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Name).IsRequired().HasMaxLength(200);
        builder.Property(ts => ts.Description).HasMaxLength(1000);
        builder.Property(ts => ts.CreatedAt).IsRequired();
        builder.Property(ts => ts.SetAccessType).IsRequired().HasConversion<string>();
        builder.Property(ts => ts.TrainingType).HasConversion<string>();
        builder.Property(ts => ts.BodyPartFocus).HasConversion<string>();
        builder.HasOne(ts => ts.CreatedBy)
               .WithMany()
               .HasForeignKey(ts => ts.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ts => ts.SetBlocks)
               .WithOne(sb => sb.TrainingSet)
               .HasForeignKey(sb => sb.TrainingSetId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
