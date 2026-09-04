using Backend.Application.Common;
using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IAIChatService
{
    Task<BaseResponse<AIChatResponse>> ChatAsync(Guid userId, AIChatRequest request);
    Task ClearHistoryAsync(Guid userId);
    Task<UserUsageDto> GetUsageAsync(Guid userId);
}
