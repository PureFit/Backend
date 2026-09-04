using Backend.Application.DTOs.Chat;

namespace Backend.Application.Services;

public interface IChatHistoryCache
{
    Task<List<ChatMessageDto>> GetAsync(Guid userId);
    Task AppendAsync(Guid userId, ChatMessageDto message);
    Task ClearAsync(Guid userId);
}
