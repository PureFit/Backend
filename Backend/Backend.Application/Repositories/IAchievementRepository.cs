using Backend.Core.Entities.AchievementRelated;

namespace Backend.Application.Repositories;

public interface IAchievementRepository
{
    Task<List<Achievement>> GetAllAsync();
    Task<List<UserAchievement>> GetByUserInfoIdAsync(Guid userInfoId);
    Task<bool> HasAchievementAsync(Guid userInfoId, Guid achievementId);
    Task AddUserAchievementAsync(UserAchievement userAchievement);

    /// <summary>Кол-во завершённых плановых тренировок (сессий с PlanTrainingId)</summary>
    Task<int> GetCompletedPlanSessionCountAsync(Guid userInfoId);

    /// <summary>Кол-во полностью завершённых недель плана</summary>
    Task<int> GetCompletedWeekCountAsync(Guid userInfoId);
}
