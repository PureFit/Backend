namespace Backend.Application.DTOs.Excercises;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? GifUrl { get; set; }
    public string Category { get; set; } = null!;
    public List<string> BodyParts { get; set; } = [];
    public List<string> PrimaryMuscles { get; set; } = [];
    public List<string> Equipments { get; set; } = [];
    public bool AllowsWeight { get; set; }
    public List<ExerciseTypeDto> ExerciseTypes { get; set; } = [];
}
