using Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserGoogleTokenConfiguration : IEntityTypeConfiguration<UserGoogleToken>
{
    public void Configure(EntityTypeBuilder<UserGoogleToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.AccessToken).IsRequired().HasMaxLength(2048);
        builder.Property(t => t.RefreshToken).IsRequired().HasMaxLength(512);
        builder.Property(t => t.ConnectedEmail).IsRequired().HasMaxLength(255);

        builder.HasOne(t => t.User)
            .WithOne(u => u.GoogleToken)
            .HasForeignKey<UserGoogleToken>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
