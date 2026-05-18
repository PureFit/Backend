using Backend.Core.Enums;

namespace Backend.Core.Entities.AchievementRelated;

public class Achievement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? IconEmoji { get; set; }
    public AchievementType Type { get; set; }

    /// <summary>
    /// JSON с параметрами условия. Структура зависит от Type:
    /// CreateFirstSet / CreatePlan  → {}
    /// FinishPlanTraining           → { "requiredCount": 3 }
    /// FinishPlanWeek               → { "requiredCount": 1 }
    /// MuscleRawVolume              → { "muscleName": "Biceps", "requiredVolume": 50000 }
    /// </summary>
    public string ConditionJson { get; set; } = "{}";

    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}
