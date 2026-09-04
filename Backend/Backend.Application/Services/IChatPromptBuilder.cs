using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IChatPromptBuilder
{
    /// <summary>
    /// Строит промпт для AI-тренера: системный контекст из профиля + плана,
    /// история переписки + текущее сообщение пользователя.
    /// </summary>
    AIPrompt Build(AIChatRequest request, UserChatContext context, List<ChatMessageDto> history);
}
