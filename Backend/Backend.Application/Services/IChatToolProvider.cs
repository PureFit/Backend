using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

/// <summary>
/// Возвращает список инструментов, которые AI может вызвать в чате.
/// </summary>
public interface IChatToolProvider
{
    IReadOnlyList<ToolDefinition> GetTools();
}
