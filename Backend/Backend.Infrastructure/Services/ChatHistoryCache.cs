using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class ChatHistoryCache : IChatHistoryCache
{
    // Keep last N messages to avoid ballooning the prompt
    private const int MaxMessages = 20;
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

    private readonly ICacheService _cache;

    public ChatHistoryCache(ICacheService cache)
    {
        _cache = cache;
    }

    public Task<List<ChatMessageDto>> GetAsync(Guid userId) =>
        _cache.GetAsync<List<ChatMessageDto>>(CacheKeys.ChatHistory(userId))
              .ContinueWith(t => t.Result ?? []);

    public async Task AppendAsync(Guid userId, ChatMessageDto message)
    {
        var history = await GetAsync(userId);
        history.Add(message);

        if (history.Count > MaxMessages)
            history = history[^MaxMessages..];

        await _cache.SetAsync(CacheKeys.ChatHistory(userId), history, Ttl);
    }

    public Task ClearAsync(Guid userId) =>
        _cache.RemoveAsync(CacheKeys.ChatHistory(userId));
}
