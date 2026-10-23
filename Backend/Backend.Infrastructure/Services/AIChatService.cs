using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class AIChatService : IAIChatService
{
    private readonly IChatHistoryCache _historyCache;
    private readonly IUsageLimiterService _usageLimiter;
    private readonly IChatToolProvider _toolProvider;
    private readonly IChatAgentLoop _agentLoop;

    public AIChatService(
        IChatHistoryCache historyCache,
        IUsageLimiterService usageLimiter,
        IChatToolProvider toolProvider,
        IChatAgentLoop agentLoop)
    {
        _historyCache = historyCache;
        _usageLimiter = usageLimiter;
        _toolProvider = toolProvider;
        _agentLoop    = agentLoop;
    }

    public async Task<BaseResponse<AIChatResponse>> ChatAsync(Guid userId, AIChatRequest request)
    {
        var denied = await _usageLimiter.CheckAsync(userId);
        if (denied.HasValue)
            return BaseResponse<AIChatResponse>.Fail(denied.Value);

        var history  = await _historyCache.GetAsync(userId);
        var tools    = _toolProvider.GetTools();
        var messages = new List<ChatMessageDto>(history)
        {
            new() { Role = "user", Content = request.Message }
        };

        var result = await _agentLoop.RunAsync(messages, tools, userId);

        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "user",      Content = request.Message });
        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "assistant", Content = result.FinalContent });
        await _usageLimiter.RecordAsync(userId, result.TotalTokens);

        return BaseResponse<AIChatResponse>.Ok(new AIChatResponse { Message = result.FinalContent });
    }

    public Task ClearHistoryAsync(Guid userId) => _historyCache.ClearAsync(userId);

    public Task<UserUsageDto> GetUsageAsync(Guid userId) => _usageLimiter.GetUsageAsync(userId);
}
