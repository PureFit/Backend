namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Внутренний реквест для пайплайна генерации — объединяет данные опросника + данные пользователя.
/// </summary>
public class GeneratePlanRequest
{
    // Данные опросника
    public string PlanType { get; set; } = null!;
    public string PlanSubType { get; set; } = null!;
    public string? GoalMetadata { get; set; }
    public int SessionsPerWeek { get; set; }
    public int SessionDurationMinutes { get; set; }
    public int PlanDurationWeeks { get; set; }
    public List<string> AvailableEquipment { get; set; } = [];
    public string? FreeTextWish { get; set; }

    // Данные пользователя (из UserInfo)
    public string? Sex { get; set; }
    public int? AgeYears { get; set; }
    public double? WeightKg { get; set; }
    public int? HeightCm { get; set; }
    public string? FitnessLevel { get; set; }
}
