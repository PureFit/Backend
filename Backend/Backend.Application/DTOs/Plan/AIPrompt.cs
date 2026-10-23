using Backend.Application.DTOs.Chat;

namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Промпт готовый к отправке в AI — system message + сообщения + (опционально) инструменты.
/// </summary>
public class AIPrompt
{
    public string SystemMessage { get; set; } = null!;

    /// <summary>
    /// Плоское текстовое сообщение пользователя.
    /// Используется для генерации плана (RequireJsonResponse=true).
    /// Если задан Messages — этот поле игнорируется.
    /// </summary>
    public string? UserMessage { get; set; }

    /// <summary>
    /// Структурированная история переписки (для chat с tool calling).
    /// Когда задан — клиент использует его вместо UserMessage.
    /// </summary>
    public List<ChatMessageDto>? Messages { get; set; }

    /// <summary>
    /// Инструменты, доступные AI. Null — инструменты не предоставляются.
    /// </summary>
    public List<ToolDefinition>? Tools { get; set; }

    // index (1,2,3...) → real exercise UUID, populated when building the catalog
    public Dictionary<int, Guid> ExerciseIndexMap { get; set; } = [];

    /// <summary>
    /// Если true — просим у провайдера JSON-режим (для генерации плана).
    /// Для свободного чата должно быть false.
    /// </summary>
    public bool RequireJsonResponse { get; set; } = false;
}

