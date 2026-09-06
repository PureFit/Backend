using Backend.Core.Entities;
using Backend.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(SubscriptionStatus.None);

        builder.Property(s => s.StripeCustomerId).HasMaxLength(255).IsRequired(false);
        builder.Property(s => s.StripeSubscriptionId).HasMaxLength(255).IsRequired(false);
        builder.Property(s => s.PriceId).HasMaxLength(255).IsRequired(false);
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasIndex(s => s.UserId).IsUnique();
        builder.HasIndex(s => s.StripeCustomerId);
        builder.HasIndex(s => s.StripeSubscriptionId);

        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<UserSubscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
