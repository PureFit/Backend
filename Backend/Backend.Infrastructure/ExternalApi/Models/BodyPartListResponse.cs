using System.Text.Json.Serialization;

namespace Backend.Infrastructure.ExternalApi.Models;

public class BodyPartListResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<BodyPartDto> Data { get; set; } = new();
}

public class BodyPartDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = null!;
}