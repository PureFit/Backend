using Backend.Application.DTOs.Chat;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class ChatAgentLoop : IChatAgentLoop
{
    private readonly IAIService _aiService;
    private readonly IChatPromptBuilder _promptBuilder;
    private readonly IChatToolExecutor _toolExecutor;

    public ChatAgentLoop(
        IAIService aiService,
        IChatPromptBuilder promptBuilder,
        IChatToolExecutor toolExecutor)
    {
        _aiService      = aiService;
        _promptBuilder  = promptBuilder;
        _toolExecutor   = toolExecutor;
    }

    public async Task<AgentLoopResult> RunAsync(
        List<ChatMessageDto> messages,
        IReadOnlyList<ToolDefinition> tools,
        Guid userId)
    {
        int totalTokens = 0;
        AIResponse response;

        while (true)
        {
            var prompt  = _promptBuilder.Build(messages, tools);
            response    = await _aiService.ChatAsync(prompt);
            totalTokens += response.TokensUsed;

            if (!response.HasToolCalls)
                break;

            messages.Add(new ChatMessageDto
            {
                Role      = "assistant",
                ToolCalls = response.ToolCalls
            });

            foreach (var call in response.ToolCalls!)
            {
                var result = await _toolExecutor.ExecuteAsync(call.Name, call.ArgumentsJson, userId);
                messages.Add(new ChatMessageDto
                {
                    Role       = "tool",
                    ToolCallId = call.Id,
                    Content    = result
                });
            }
        }

        return new AgentLoopResult(response.Content!, totalTokens);
    }
}
