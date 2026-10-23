using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class ChatPromptBuilder : IChatPromptBuilder
{
    public string BuildSystemMessage() =>
        """
        You are PureFit AI Trainer — a knowledgeable, supportive personal trainer and nutrition coach.
        Answer questions about workouts, exercise technique, nutrition, recovery, and fitness goals.
        Keep responses practical, concise, and tailored to the user's profile.
        If you need the user's profile or training plan to answer — use the available tools to fetch it.
        Only call tools when the question actually requires that data.
        """;

    public AIPrompt Build(List<ChatMessageDto> messages, IReadOnlyList<ToolDefinition>? tools = null) =>
        new()
        {
            SystemMessage = BuildSystemMessage(),
            Messages      = messages,
            Tools         = tools?.ToList()
        };
}
