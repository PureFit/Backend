namespace Backend.Application.Common;

public class GeminiSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "gemini-2.0-flash";
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 4096;
}
