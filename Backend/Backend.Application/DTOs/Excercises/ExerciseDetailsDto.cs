namespace Backend.Application.DTOs.Excercises;

public class ExerciseDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? GifUrl { get; set; }
    public string? Overview { get; set; }
    public string Category { get; set; } = null!;
    public bool AllowsWeight { get; set; }
    public float? BaseWeightBodyRatio { get; set; }
    public List<string> BodyParts { get; set; } = [];
    public List<string> PrimaryMuscles { get; set; } = [];
    public List<string> SecondaryMuscles { get; set; } = [];
    public List<string> Equipments { get; set; } = [];
    public List<ExerciseTypeDto> ExerciseTypes { get; set; } = [];
    public List<string> Instructions { get; set; } = [];
    public List<string> Tips { get; set; } = [];
    public List<string> Variations { get; set; } = [];
    public List<string> Keywords { get; set; } = [];
    public List<ExerciseDto> RelatedExercises { get; set; } = [];
}
