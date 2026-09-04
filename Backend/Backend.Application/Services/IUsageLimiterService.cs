using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IUsageLimiterService
{
    /// <summary>
    /// Проверяет лимиты. Возвращает null если разрешено, иначе — причину отказа.
    /// </summary>
    Task<string?> CheckAsync(Guid userId);

    Task RecordAsync(Guid userId, int tokensUsed);

    Task<UserUsageDto> GetUsageAsync(Guid userId);
}
