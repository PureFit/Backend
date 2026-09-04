namespace Backend.Application.DTOs.Chat;

public class AIChatRequest
{
    public string Message { get; set; } = null!;
}

public class ChatMessageDto
{
    /// <summary>user or assistant</summary>
    public string Role { get; set; } = null!;
    public string Content { get; set; } = null!;
}
