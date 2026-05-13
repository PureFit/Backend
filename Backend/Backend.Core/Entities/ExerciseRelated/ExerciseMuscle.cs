using Backend.Core.Enums;

namespace Backend.Core.Entities.ExerciseRelated;

public class ExerciseMuscle
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid MuscleId { get; set; }
    public Muscle Muscle { get; set; } = null!;

    public MuscleRole Role { get; set; }
}
