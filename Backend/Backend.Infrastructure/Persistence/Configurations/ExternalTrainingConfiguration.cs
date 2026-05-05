using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class ExternalTrainingConfiguration : IEntityTypeConfiguration<ExternalTraining>
{
    public void Configure(EntityTypeBuilder<ExternalTraining> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Start).IsRequired();
        builder.Property(e => e.PerceivedExertion).HasConversion<string>();
        builder.Property(e => e.GoogleEventId).HasMaxLength(200);

        // Основная привязка к пользователю — обязательная.
        builder.HasOne(e => e.UserInfo)
               .WithMany()
               .HasForeignKey(e => e.UserInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        // Привязка к неделе плана — опциональная.
        builder.HasOne(e => e.WeekPlan)
               .WithMany(w => w.ExternalTrainings)
               .HasForeignKey(e => e.WeekPlanId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
