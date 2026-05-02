using Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserMetricLogConfiguration : IEntityTypeConfiguration<UserMetricLog>
{
    public void Configure(EntityTypeBuilder<UserMetricLog> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Metric).IsRequired().HasConversion<string>();
        builder.Property(m => m.Value).IsRequired().HasMaxLength(100);
        builder.Property(m => m.LoggedAt).IsRequired();

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.UserId, m.Metric, m.LoggedAt });
    }
}
