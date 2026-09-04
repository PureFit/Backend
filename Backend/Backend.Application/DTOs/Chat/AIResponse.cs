namespace Backend.Application.DTOs.Chat;

public class AIResponse
{
    public string Content { get; set; } = null!;
    public int TokensUsed { get; set; }
}
