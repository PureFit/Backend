namespace Backend.Application.DTOs.Chat;

/// <summary>
/// Контекст пользователя, который передаётся в IChatPromptBuilder
/// для обогащения системного промпта.
/// </summary>
public class UserChatContext
{
    // Profile
    public string? Sex { get; set; }
    public int? AgeYears { get; set; }
    public decimal? WeightKg { get; set; }
    public int? HeightCm { get; set; }
    public string? FitnessLevel { get; set; }

    // Active plan (null if no plan)
    public ActivePlanSummary? ActivePlan { get; set; }
}

public class ActivePlanSummary
{
    public string PlanType { get; set; } = null!;
    public string PlanSubType { get; set; } = null!;
    public int WeeksDuration { get; set; }
    public int CurrentWeek { get; set; }
    public List<UpcomingTrainingSummary> UpcomingTrainings { get; set; } = [];
}

public class UpcomingTrainingSummary
{
    public string PlannedDate { get; set; } = null!;
    public string? TrainingSetName { get; set; }
    public bool IsCompleted { get; set; }
}
