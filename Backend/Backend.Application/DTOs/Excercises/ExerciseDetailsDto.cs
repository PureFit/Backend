namespace Backend.Application.DTOs.Excercises;

public class ExerciseDetailsDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string? ImageUrl480p { get; set; }
    public string? ImageUrl720p { get; set; }
    public string? VideoUrl { get; set; }
    public string Overview { get; set; } = null!;
    public List<string> BodyParts { get; set; } = new();
    public List<string> TargetMuscles { get; set; } = new();
    public List<string> SecondaryMuscles { get; set; } = new();
    public List<string> Equipments { get; set; } = new();
    public string ExerciseType { get; set; } = null!;
    public List<string> Instructions { get; set; } = new();
    public List<string> ExerciseTips { get; set; } = new();
    public List<string> Variations { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
}
