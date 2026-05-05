using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.Sex).IsRequired().HasConversion<string>();
        builder.Property(ui => ui.Level).IsRequired().HasConversion<string>();
        builder.Property(ui => ui.WeightKg).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(ui => ui.HeightCm).IsRequired();
        builder.Property(ui => ui.DateOfBirth).IsRequired();

        builder.HasMany(ui => ui.Plans)
               .WithOne(p => p.UserInfo)
               .HasForeignKey(p => p.UserInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        // CurrentPlan — опциональная ссылка, NoAction чтобы не было цикла с Plans.
        builder.HasOne(ui => ui.CurrentPlan)
               .WithMany()
               .HasForeignKey(ui => ui.CurrentPlanId)
               .OnDelete(DeleteBehavior.NoAction);

        // WorkloadStats и Sessions настроены в их конфигах.
    }
}
