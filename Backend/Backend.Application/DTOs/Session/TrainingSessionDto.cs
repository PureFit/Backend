using Backend.Core.Enums;

namespace Backend.Application.DTOs.Session;

public class TrainingSessionDto
{
    public Guid Id { get; set; }
    public SessionStatus Status { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public int? DurationMinutes { get; set; }
    public Guid TrainingSetId { get; set; }
    public string TrainingSetName { get; set; } = "";
    public Guid? PlanTrainingId { get; set; }
}

public class StartSessionRequest
{
    public Guid TrainingSetId { get; set; }
    public Guid? PlanTrainingId { get; set; }
}

public class SessionHistoryResponse
{
    public List<TrainingSessionDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}
