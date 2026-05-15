using System.Text.Json.Serialization;

namespace Backend.Infrastructure.Services;

internal class GroqRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("messages")]
    public List<GroqMessage> Messages { get; set; } = [];

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("response_format")]
    public GroqResponseFormat ResponseFormat { get; set; } = new();
}

internal class GroqMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}

internal class GroqResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "json_object";
}

internal class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<GroqChoice> Choices { get; set; } = [];
}

internal class GroqChoice
{
    [JsonPropertyName("message")]
    public GroqMessage Message { get; set; } = null!;
}
