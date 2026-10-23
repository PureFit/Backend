namespace Backend.Application.DTOs.Chat;

/// <summary>
/// Вызов инструмента от AI (возвращается в AIResponse.ToolCalls).
/// </summary>
public class ToolCallDto
{
    /// <summary>Уникальный ID вызова — нужен при отправке результата обратно.</summary>
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    /// <summary>JSON-строка с аргументами вызова.</summary>
    public string ArgumentsJson { get; set; } = "{}";
}
