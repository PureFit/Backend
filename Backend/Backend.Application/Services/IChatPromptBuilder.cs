using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

/// <summary>
/// Собирает AIPrompt для чата. Данные пользователя не инжектируются заранее —
/// AI запрашивает их через инструменты по мере необходимости.
/// </summary>
public interface IChatPromptBuilder
{
    /// <summary>
    /// Системный промпт без данных пользователя.
    /// Кешируется на стороне AI-провайдера (prompt caching).
    /// </summary>
    string BuildSystemMessage();

    /// <summary>
    /// Строит AIPrompt из структурированного списка сообщений и доступных инструментов.
    /// </summary>
    AIPrompt Build(List<ChatMessageDto> messages, IReadOnlyList<ToolDefinition>? tools = null);
}
