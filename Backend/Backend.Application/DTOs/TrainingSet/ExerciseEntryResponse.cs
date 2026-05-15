namespace Backend.Application.DTOs.TrainingSet;

public class ExerciseEntryResponse
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string? ExerciseName { get; set; }
    public Guid ExerciseTypeId { get; set; }
    public int Order { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }
    public int RestAfterCurrentEntrySeconds { get; set; }
    public List<ExerciseIntervalResponse> Intervals { get; set; } = [];
}
