using Backend.Application.DTOs.Achievement;

namespace Backend.Application.Services;

/// <summary>
/// Абстракция отправки real-time уведомлений.
/// Реализуется в API-слое через SignalR IHubContext.
/// </summary>
public interface IAchievementNotifier
{
    Task NotifyAsync(string userId, AchievementDto dto);
}
