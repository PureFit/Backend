using System.Text.Json.Serialization;

namespace Backend.Infrastructure.ExternalApi.Models;

public class DetailedExerciseResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public DetailedExerciseDto Data { get; set; } = null!;
}

public class DetailedExerciseDto
{
    [JsonPropertyName("exerciseId")]
    public string ExerciseId { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = null!;

    [JsonPropertyName("imageUrls")]
    public ImageResolutions ImageUrls { get; set; } = null!;

    [JsonPropertyName("equipments")]
    public List<string> Equipments { get; set; } = new();

    [JsonPropertyName("bodyParts")]
    public List<string> BodyParts { get; set; } = new();

    [JsonPropertyName("exerciseType")]
    public string ExerciseType { get; set; } = null!;

    [JsonPropertyName("targetMuscles")]
    public List<string> TargetMuscles { get; set; } = new();

    [JsonPropertyName("secondaryMuscles")]
    public List<string> SecondaryMuscles { get; set; } = new();

    [JsonPropertyName("videoUrl")]
    public string? VideoUrl { get; set; }

    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = new();

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = null!;

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();

    [JsonPropertyName("exerciseTips")]
    public List<string> ExerciseTips { get; set; } = new();

    [JsonPropertyName("variations")]
    public List<string> Variations { get; set; } = new();

    [JsonPropertyName("relatedExerciseIds")]
    public List<string> RelatedExerciseIds { get; set; } = new();
}

public class ImageResolutions
{
    [JsonPropertyName("360p")]
    public string? P360 { get; set; }

    [JsonPropertyName("480p")]
    public string? P480 { get; set; }

    [JsonPropertyName("720p")]
    public string? P720 { get; set; }

    [JsonPropertyName("1080p")]
    public string? P1080 { get; set; }
}