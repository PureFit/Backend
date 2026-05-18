using Backend.Core.Entities.AchievementRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.IconEmoji).HasMaxLength(10);
        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
        builder.Property(a => a.ConditionJson).IsRequired().HasColumnType("jsonb");
    }
}
