namespace Backend.Application.DTOs.Chat;

public class AIChatRequest
{
    public string Message { get; set; } = null!;
}

public class ChatMessageDto
{
    /// <summary>user | assistant | tool</summary>
    public string Role { get; set; } = null!;

    /// <summary>Текстовое содержимое. Null когда role=assistant и есть ToolCalls.</summary>
    public string? Content { get; set; }

    /// <summary>Вызовы инструментов. Заполняется когда role=assistant.</summary>
    public List<ToolCallDto>? ToolCalls { get; set; }

    /// <summary>ID вызова инструмента. Заполняется когда role=tool.</summary>
    public string? ToolCallId { get; set; }
}
