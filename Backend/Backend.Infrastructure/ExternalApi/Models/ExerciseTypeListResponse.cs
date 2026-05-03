using System.Text.Json.Serialization;

namespace Backend.Infrastructure.ExternalApi.Models;

public class ExerciseTypeListResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<ExerciseTypeDto> Data { get; set; } = new();
}

public class ExerciseTypeDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
