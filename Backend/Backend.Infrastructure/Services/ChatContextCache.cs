using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class ChatContextCache : IChatContextCache
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    private readonly ICacheService _cache;

    public ChatContextCache(ICacheService cache)
    {
        _cache = cache;
    }

    public Task<UserChatContext?> GetAsync(Guid userId) =>
        _cache.GetAsync<UserChatContext>(CacheKeys.ChatContext(userId));

    public Task SetAsync(Guid userId, UserChatContext context) =>
        _cache.SetAsync(CacheKeys.ChatContext(userId), context, Ttl);

    public Task InvalidateAsync(Guid userId) =>
        _cache.RemoveAsync(CacheKeys.ChatContext(userId));
}
