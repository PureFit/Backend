using Backend.Application.Common;
using Backend.Application.DTOs.Achievement;
using Backend.Application.Repositories;
using Backend.Core.Entities.AchievementRelated;
using Backend.Core.Enums;
using Newtonsoft.Json;

namespace Backend.Application.Services.impl;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _repo;
    private readonly IUserInfoRepository _userInfoRepo;
    private readonly IAchievementNotifier _notifier;

    public AchievementService(
        IAchievementRepository repo,
        IUserInfoRepository userInfoRepo,
        IAchievementNotifier notifier)
    {
        _repo = repo;
        _userInfoRepo = userInfoRepo;
        _notifier = notifier;
    }

    public async Task<BaseResponse<List<AchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
        var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
        if (userInfo is null)
            return BaseResponse<List<AchievementDto>>.Fail(ErrorEnums.UserNotFound);

        var all = await _repo.GetAllAsync();
        var unlocked = await _repo.GetByUserInfoIdAsync(userInfo.Id);
        var unlockedMap = unlocked.ToDictionary(ua => ua.AchievementId, ua => ua.AchievedAt);

        var dtos = all.Select(a => new AchievementDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            IconEmoji = a.IconEmoji,
            IsUnlocked = unlockedMap.ContainsKey(a.Id),
            AchievedAt = unlockedMap.TryGetValue(a.Id, out var dt) ? dt : null
        }).ToList();

        return BaseResponse<List<AchievementDto>>.Ok(dtos);
    }

    public async Task CheckAndGrantAsync(Guid userId, AchievementType type)
    {
        var userInfo = await _userInfoRepo.GetByUserIdWithStatsAsync(userId);
        if (userInfo is null) return;

        var achievements = (await _repo.GetAllAsync()).Where(a => a.Type == type);

        foreach (var achievement in achievements)
        {
            bool conditionMet = achievement.Type switch
            {
                AchievementType.CreateFirstSet or AchievementType.CreatePlan =>
                    true,

                AchievementType.FinishPlanTraining =>
                    JsonConvert.DeserializeObject<CountCondition>(achievement.ConditionJson) is { } cc &&
                    await _repo.GetCompletedPlanSessionCountAsync(userInfo.Id) >= cc.RequiredCount,

                AchievementType.FinishPlanWeek =>
                    JsonConvert.DeserializeObject<CountCondition>(achievement.ConditionJson) is { } cw &&
                    await _repo.GetCompletedWeekCountAsync(userInfo.Id) >= cw.RequiredCount,

                AchievementType.MuscleRawVolume =>
                    JsonConvert.DeserializeObject<MuscleVolumeCondition>(achievement.ConditionJson) is { } mvc &&
                    userInfo.WorkloadStats.Any(s => s.Name == mvc.MuscleName && s.AccumulatedVolume >= mvc.RequiredVolume),

                _ => false
            };

            if (!conditionMet) continue;
            if (await _repo.HasAchievementAsync(userInfo.Id, achievement.Id)) continue;

            var achievedAt = DateTime.UtcNow;

            await _repo.AddUserAchievementAsync(new UserAchievement
            {
                Id = Guid.NewGuid(),
                UserInfoId = userInfo.Id,
                AchievementId = achievement.Id,
                AchievedAt = achievedAt
            });

            await _notifier.NotifyAsync(userId.ToString(), new AchievementDto
            {
                Id = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                IconEmoji = achievement.IconEmoji,
                IsUnlocked = true,
                AchievedAt = achievedAt
            });
        }
    }
}
