namespace Backend.Core.Entities.ExerciseRelated;

public class Muscle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public Guid BodyPartId { get; set; }
    public BodyPart BodyPart { get; set; } = null!;

    public List<ExerciseMuscle> ExerciseMuscles { get; set; } = [];
}
