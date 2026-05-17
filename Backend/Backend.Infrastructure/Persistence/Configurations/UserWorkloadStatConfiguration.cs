using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserWorkloadStatConfiguration : IEntityTypeConfiguration<UserWorkloadStat>
{
    public void Configure(EntityTypeBuilder<UserWorkloadStat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Category).IsRequired().HasConversion<string>();
        builder.Property(s => s.AccumulatedVolume).IsRequired();
        builder.Property(s => s.SessionCount).IsRequired();
        builder.Property(s => s.LastUpdatedAt).IsRequired();

        builder.HasOne(s => s.UserInfo)
               .WithMany(u => u.WorkloadStats)
               .HasForeignKey(s => s.UserInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        // Уникальность: один стат на пару (UserInfo, Name, Category).
        builder.HasIndex(s => new { s.UserInfoId, s.Name, s.Category }).IsUnique();
    }
}
