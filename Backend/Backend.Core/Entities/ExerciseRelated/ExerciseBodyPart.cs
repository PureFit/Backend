namespace Backend.Core.Entities.ExerciseRelated;

public class ExerciseBodyPart
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid BodyPartId { get; set; }
    public BodyPart BodyPart { get; set; } = null!;
}
