using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<UserMetricLog> UserMetricLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserGoogleToken> UserGoogleTokens { get; set; }

    public DbSet<AIPlan> AiPlans { get; set; }
    public DbSet<WeekPlan> WeekPlans { get; set; }
    public DbSet<PlanTraining> PlanTrainings { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<TrainingSet> TrainingSets { get; set; }
    public DbSet<SetBlock> SetBlocks { get; set; }
    public DbSet<ExerciseEntry> ExerciseEntries { get; set; }
    public DbSet<ExerciseInterval> ExerciseIntervals { get; set; }
    public DbSet<ExternalTraining> ExternalTrainings { get; set; }
    public DbSet<UserWorkloadStat> UserWorkloadStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
