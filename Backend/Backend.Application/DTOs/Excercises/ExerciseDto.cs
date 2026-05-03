namespace Backend.Application.DTOs.Excercises;

public class ExerciseDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public List<string> BodyParts { get; set; } = new();
    public List<string> TargetMuscles { get; set; } = new();
    public List<string> Equipments { get; set; } = new();
    public string ExerciseType { get; set; } = null!;
}
