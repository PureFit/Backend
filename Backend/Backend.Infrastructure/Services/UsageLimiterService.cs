using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Services;

public class UsageLimiterService : IUsageLimiterService
{
    private readonly ICacheService _cache;
    private readonly UsageLimitsConfig _limits;

    public UsageLimiterService(ICacheService cache, IOptions<UsageLimitsConfig> limits)
    {
        _cache  = cache;
        _limits = limits.Value;
    }

    public async Task<ErrorEnums?> CheckAsync(Guid userId)
    {
        if (!_limits.ChatEnabled)
            return ErrorEnums.RateLimitExceeded;

        var usage = await GetUsageAsync(userId);

        if (usage.IsRequestLimitReached || usage.IsTokenLimitReached)
            return ErrorEnums.RateLimitExceeded;

        return null;
    }

    public async Task RecordAsync(Guid userId, int tokensUsed)
    {
        var (tokenKey, requestKey) = Keys(userId);
        var ttl = TimeSpan.FromHours(25);

        var currentTokens   = await _cache.GetAsync<int>(tokenKey);
        var currentRequests = await _cache.GetAsync<int>(requestKey);

        await _cache.SetAsync(tokenKey,   currentTokens   + tokensUsed, ttl);
        await _cache.SetAsync(requestKey, currentRequests + 1,          ttl);
    }

    public async Task<UserUsageDto> GetUsageAsync(Guid userId)
    {
        var (tokenKey, requestKey) = Keys(userId);

        return new UserUsageDto
        {
            TokensUsedToday   = await _cache.GetAsync<int>(tokenKey),
            RequestsToday     = await _cache.GetAsync<int>(requestKey),
            DailyTokenLimit   = _limits.DailyTokenLimit,
            DailyRequestLimit = _limits.DailyRequestLimit
        };
    }

    private static (string tokenKey, string requestKey) Keys(Guid userId)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        return (CacheKeys.DailyTokens(userId, today), CacheKeys.DailyRequests(userId, today));
    }
}
