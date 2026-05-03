using System.Text.Json.Serialization;

namespace Backend.Infrastructure.ExternalApi.Models;

public class MuscleListResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<MuscleDto> Data { get; set; } = new();
}

public class MuscleDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}