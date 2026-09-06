namespace Backend.Application.Common;

public class GeminiSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "gemini-2.0-flash";
    public string EmbeddingModel { get; set; } = "gemini-embedding-exp-03-07";
    public float Temperature { get; set; } = 0.2f;
    public int MaxTokens { get; set; } = 65536;
}
