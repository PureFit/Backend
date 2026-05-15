namespace Backend.Application.DTOs.Plan;

public class ScheduledPlanDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<ScheduledWeekDto> Weeks { get; set; } = [];
}

public class ScheduledWeekDto
{
    public int WeekNumber { get; set; }
    public string Description { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<ScheduledTrainingDto> Trainings { get; set; } = [];
}

public class ScheduledTrainingDto
{
    public int TrainingNumber { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartPlannedDate { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public List<SetBlockFullDto> Blocks { get; set; } = [];
}
