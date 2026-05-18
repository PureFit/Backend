namespace Backend.Core.Entities.AchievementRelated;

/// <summary>CreateFirstSet, CreatePlan — условий нет</summary>
public class EmptyCondition { }

/// <summary>FinishPlanTraining, FinishPlanWeek</summary>
public class CountCondition
{
    public int RequiredCount { get; set; }
}

/// <summary>MuscleRawVolume</summary>
public class MuscleVolumeCondition
{
    public string MuscleName { get; set; } = null!;
    public float RequiredVolume { get; set; }
}
