using Backend.Core.Entities;
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
        builder.Property(ui => ui.Age).IsRequired();
    }
}
