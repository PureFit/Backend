using Backend.Core.Entities.AchievementRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.AchievedAt).IsRequired();

        builder.HasOne(ua => ua.UserInfo)
               .WithMany(u => u.UserAchievements)
               .HasForeignKey(ua => ua.UserInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ua => ua.Achievement)
               .WithMany(a => a.UserAchievements)
               .HasForeignKey(ua => ua.AchievementId)
               .OnDelete(DeleteBehavior.Cascade);

        // Один юзер не может получить одну ачивку дважды
        builder.HasIndex(ua => new { ua.UserInfoId, ua.AchievementId }).IsUnique();
    }
}
