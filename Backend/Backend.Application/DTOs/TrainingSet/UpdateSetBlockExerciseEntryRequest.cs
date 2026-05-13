namespace Backend.Application.DTOs.TrainingSet;

public class UpdateSetBlockExerciseEntryRequest
{
    public Guid EntryId { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }
    public int? RestAfterCurrentEntrySeconds { get; set; }
    public int? Order { get; set; }
}
