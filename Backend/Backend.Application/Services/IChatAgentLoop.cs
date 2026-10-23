using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IChatAgentLoop
{
    Task<AgentLoopResult> RunAsync(
        List<ChatMessageDto> messages,
        IReadOnlyList<ToolDefinition> tools,
        Guid userId);
}
