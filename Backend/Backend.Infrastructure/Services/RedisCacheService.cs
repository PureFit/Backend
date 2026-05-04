using System.Text.Json;
using Backend.Application.Common;
using Backend.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly RedisConfig _config;

    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger,
        IOptions<RedisConfig> options)
    {
        _cache = cache;
        _logger = logger;
        _config = options.Value;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes == null || bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(bytes);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Cache deserialize failed: {Key}", key);
            await RemoveAsync(key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache get failed: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiration ?? TimeSpan.FromMinutes(_config.DefaultExpirationMinutes)
            };
            await _cache.SetAsync(key, bytes, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache set failed: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache remove failed: {Key}", key);
        }
    }

}
