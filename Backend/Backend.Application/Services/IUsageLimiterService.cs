using Backend.Application.Common;
using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IUsageLimiterService
{
    /// <summary>
    /// Проверяет лимиты. Возвращает null если разрешено, иначе — ErrorEnums для передачи в BaseResponse.
    /// </summary>
    Task<ErrorEnums?> CheckAsync(Guid userId);

    Task RecordAsync(Guid userId, int tokensUsed);

    Task<UserUsageDto> GetUsageAsync(Guid userId);
}
