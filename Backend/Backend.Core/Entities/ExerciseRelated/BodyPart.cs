namespace Backend.Core.Entities.ExerciseRelated;

public class BodyPart
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public List<Muscle> Muscles { get; set; } = [];
    public List<ExerciseBodyPart> ExerciseBodyParts { get; set; } = [];
}
