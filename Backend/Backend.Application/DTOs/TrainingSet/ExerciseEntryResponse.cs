namespace Backend.Application.DTOs.TrainingSet;

public class ExerciseEntryResponse
{
    public Guid Id { get; set; }
    public string ExerciseId { get; set; } = null!;
    public int Order { get; set; }
    public string MeasureType { get; set; } = null!;
    public List<string> TargetMuscles { get; set; } = [];
    public List<string> SecondaryMuscles { get; set; } = [];
    public List<string> BodyParts { get; set; } = [];
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public Dictionary<string, float>? Parameters { get; set; }
    public int RestAfterCurrentEntrySeconds { get; set; }
    public List<ExerciseIntervalResponse> Intervals { get; set; } = [];
}
