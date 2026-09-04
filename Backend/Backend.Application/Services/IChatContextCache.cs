using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IChatContextCache
{
    Task<UserChatContext?> GetAsync(Guid userId);
    Task SetAsync(Guid userId, UserChatContext context);
    Task InvalidateAsync(Guid userId);
}
