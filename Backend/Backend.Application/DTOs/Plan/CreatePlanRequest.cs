namespace Backend.Application.DTOs.Plan;

public class CreatePlanRequest
{
    public string PlanType { get; set; } = null!;
    public string PlanSubType { get; set; } = null!;
    public string? GoalMetadata { get; set; }      // JSON строка, может быть null/пустой
    public int SessionsPerWeek { get; set; }
    public int SessionDurationMinutes { get; set; }
    public List<string> AvailableEquipment { get; set; } = [];
    public int PlanDurationWeeks { get; set; }
    public string? FreeTextWish { get; set; }
}
