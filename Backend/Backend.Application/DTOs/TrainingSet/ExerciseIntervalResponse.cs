namespace Backend.Application.DTOs.TrainingSet;

public class ExerciseIntervalResponse
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public int DurationSeconds { get; set; }
    public int? Reps { get; set; }
    public Dictionary<string, float>? Parameters { get; set; }
}
