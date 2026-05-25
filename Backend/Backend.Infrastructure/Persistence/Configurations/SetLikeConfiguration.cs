using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class SetLikeConfiguration : IEntityTypeConfiguration<SetLike>
{
    public void Configure(EntityTypeBuilder<SetLike> builder)
    {
        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.TrainingSet)
               .WithMany()
               .HasForeignKey(l => l.TrainingSetId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.User)
               .WithMany()
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => new { l.TrainingSetId, l.UserId }).IsUnique();
    }
}
