using System.Text.Json.Serialization;

namespace Backend.Infrastructure.Services;

internal class GeminiRequest
{
    [JsonPropertyName("system_instruction")]
    public GeminiSystemInstruction SystemInstruction { get; set; } = null!;

    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = [];

    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig GenerationConfig { get; set; } = null!;
}

internal class GeminiSystemInstruction
{
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = [];
}

internal class GeminiContent
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = [];
}

internal class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;
}

internal class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; }

    [JsonPropertyName("responseMimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResponseMimeType { get; set; }

    [JsonPropertyName("thinkingConfig")]
    public GeminiThinkingConfig ThinkingConfig { get; set; } = new();
}

internal class GeminiThinkingConfig
{
    [JsonPropertyName("thinkingBudget")]
    public int ThinkingBudget { get; set; } = 0;
}

internal class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate> Candidates { get; set; } = [];

    [JsonPropertyName("usageMetadata")]
    public GeminiUsageMetadata? UsageMetadata { get; set; }
}

internal class GeminiUsageMetadata
{
    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; set; }
}

internal class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent Content { get; set; } = null!;

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; set; }
}
