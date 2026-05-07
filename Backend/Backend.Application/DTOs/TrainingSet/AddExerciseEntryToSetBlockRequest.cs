using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class AddExerciseEntryToSetBlockRequest
{
    public Guid SetId { get; set; }
    public Guid BlockId { get; set; }
    public Guid UserId { get; set; } // set from JWT claims in controller
    public string ExerciseId { get; set; } = null!;
    public MeasureType MeasureType { get; set; }
    public List<string> TargetMuscles { get; set; } = [];
    public List<string> SecondaryMuscles { get; set; } = [];
    public List<string> BodyParts { get; set; } = [];
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public Dictionary<string, float>? Parameters { get; set; }
    public int RestAfterCurrentEntrySeconds { get; set; }
}
