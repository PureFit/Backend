using Backend.Application.DTOs.Achievement;
using Backend.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class SignalRAchievementNotifier : IAchievementNotifier
{
    private readonly IHubContext<AchievementHub> _hubContext;
    public SignalRAchievementNotifier(IHubContext<AchievementHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task NotifyAsync(string userId, AchievementDto dto)
    {
        var group = AchievementHub.UserGroup(userId);
        await _hubContext.Clients.Group(group).SendAsync(AchievementHub.AchievementUnlocked, dto);
    }
}
