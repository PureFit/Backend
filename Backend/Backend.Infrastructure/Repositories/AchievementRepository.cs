using Backend.Application.Repositories;
using Backend.Core.Entities.AchievementRelated;
using Backend.Core.Enums;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly AppDbContext _db;

    public AchievementRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Achievement>> GetAllAsync()
        => await _db.Achievements.ToListAsync();

    public async Task<List<UserAchievement>> GetByUserInfoIdAsync(Guid userInfoId)
        => await _db.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserInfoId == userInfoId)
            .ToListAsync();

    public async Task<bool> HasAchievementAsync(Guid userInfoId, Guid achievementId)
        => await _db.UserAchievements
            .AnyAsync(ua => ua.UserInfoId == userInfoId && ua.AchievementId == achievementId);

    public async Task AddUserAchievementAsync(UserAchievement userAchievement)
    {
        _db.UserAchievements.Add(userAchievement);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetCompletedPlanSessionCountAsync(Guid userInfoId)
        => await _db.TrainingSessions
            .CountAsync(s => s.UserInfoId == userInfoId
                && s.PlanTrainingId != null
                && s.Status == SessionStatus.Completed);

    public async Task<int> GetCompletedWeekCountAsync(Guid userInfoId)
    {
        var weeks = await _db.WeekPlans
            .Where(w => w.AIPlan.UserInfoId == userInfoId)
            .Include(w => w.PlanTrainings)
                .ThenInclude(t => t.TrainingSession)
            .ToListAsync();

        return weeks.Count(w =>
            w.PlanTrainings.Any() &&
            w.PlanTrainings.All(t =>
                t.TrainingSession != null &&
                t.TrainingSession.Status == SessionStatus.Completed));
    }
}
