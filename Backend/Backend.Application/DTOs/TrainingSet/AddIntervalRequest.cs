namespace Backend.Application.DTOs.TrainingSet;

public class AddIntervalRequest
{
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }
}
