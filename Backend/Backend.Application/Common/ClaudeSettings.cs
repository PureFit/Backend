namespace Backend.Application.Common;

public class ClaudeSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "claude-sonnet-4-6";
    public float Temperature { get; set; } = 0.2f;
    public int MaxTokens { get; set; } = 16000;
}
