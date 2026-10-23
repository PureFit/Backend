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

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 32768;

    [JsonPropertyName("response_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GroqResponseFormat? ResponseFormat { get; set; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GroqTool>? Tools { get; set; }
}

internal class GroqMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; set; }

    [JsonPropertyName("tool_calls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GroqToolCall>? ToolCalls { get; set; }

    [JsonPropertyName("tool_call_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolCallId { get; set; }
}

internal class GroqTool
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";

    [JsonPropertyName("function")]
    public GroqFunction Function { get; set; } = null!;
}

internal class GroqFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("parameters")]
    public object Parameters { get; set; } = null!;
}

internal class GroqToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";

    [JsonPropertyName("function")]
    public GroqToolCallFunction Function { get; set; } = null!;
}

internal class GroqToolCallFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = "{}";
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

    [JsonPropertyName("usage")]
    public GroqUsage? Usage { get; set; }
}

internal class GroqUsage
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

internal class GroqChoice
{
    [JsonPropertyName("message")]
    public GroqMessage Message { get; set; } = null!;

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}
