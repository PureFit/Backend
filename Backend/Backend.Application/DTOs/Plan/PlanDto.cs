namespace Backend.Application.DTOs.Plan;

public class PlanDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int WeeksDuration { get; set; }
    public string PlanType { get; set; } = null!;
    public string PlanSubType { get; set; } = null!;
    public List<WeekPlanDto> Weeks { get; set; } = [];
}

public class WeekPlanDto
{
    public Guid Id { get; set; }
    public int WeekNumber { get; set; }
    public string Description { get; set; } = null!;
    public string StartDate { get; set; } = null!;
    public string EndDate { get; set; } = null!;
    public string WeekStatus { get; set; } = null!;
    public List<PlanTrainingDto> PlanTrainings { get; set; } = [];
}

public class PlanTrainingDto
{
    public Guid Id { get; set; }
    public int TrainingNumber { get; set; }
    public string Description { get; set; } = null!;
    public string StartPlannedDate { get; set; } = null!;
    public TrainingSetSummaryDto? TrainingSet { get; set; }
}

public class TrainingSetSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
