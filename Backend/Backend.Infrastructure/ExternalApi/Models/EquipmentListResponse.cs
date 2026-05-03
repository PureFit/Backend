using System.Text.Json.Serialization;

namespace Backend.Infrastructure.ExternalApi.Models;

public class EquipmentListResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<EquipmentDto> Data { get; set; } = new();
}

public class EquipmentDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = null!;
}