namespace Backend.Application.DTOs.Chat;

public class AIResponse
{
    /// <summary>Текст ответа. Null когда AI вернул tool calls вместо текста.</summary>
    public string? Content { get; set; }

    public int TokensUsed { get; set; }

    /// <summary>Запросы к инструментам от AI. Не null → нужно выполнить и отправить результаты.</summary>
    public List<ToolCallDto>? ToolCalls { get; set; }

    public bool HasToolCalls => ToolCalls?.Count > 0;
}
