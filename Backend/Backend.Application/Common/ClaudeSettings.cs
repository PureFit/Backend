namespace Backend.Application.Common;

public class ClaudeSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "claude-opus-4-6";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 4096;
}
