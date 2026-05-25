using Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class UserFriendshipConfiguration : IEntityTypeConfiguration<UserFriendship>
{
    public void Configure(EntityTypeBuilder<UserFriendship> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.HasOne(f => f.Requester)
               .WithMany()
               .HasForeignKey(f => f.RequesterId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Addressee)
               .WithMany()
               .HasForeignKey(f => f.AddresseeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.RequesterId, f.AddresseeId }).IsUnique();
    }
}
