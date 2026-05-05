using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        // Основная привязка к профилю пользователя — обязательная.
        builder.HasOne(s => s.UserInfo)
               .WithMany(u => u.Sessions)
               .HasForeignKey(s => s.UserInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        // Привязка к плановой тренировке — опциональная.
        builder.HasOne(s => s.PlanTraining)
               .WithOne(p => p.TrainingSession)
               .HasForeignKey<TrainingSession>(s => s.PlanTrainingId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
