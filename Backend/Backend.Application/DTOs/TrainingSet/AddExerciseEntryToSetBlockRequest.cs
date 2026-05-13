using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class AddExerciseEntryToSetBlockRequest
{
    public Guid BlockId { get; set; }
    public Guid UserId { get; set; } // set from JWT claims in controller
    public Guid ExerciseId { get; set; }
    public Guid ExerciseTypeId { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }
    public int RestAfterCurrentEntrySeconds { get; set; }
    public List<AddIntervalRequest> Intervals { get; set; } = [];
}
