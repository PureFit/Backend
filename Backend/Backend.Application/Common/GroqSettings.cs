namespace Backend.Application.Common;

public class GroqSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "llama-3.3-70b-versatile";
    public float Temperature { get; set; } = 0.7f;
}
