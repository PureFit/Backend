using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

[Authorize]
public class AchievementHub : Hub
{
    public const string AchievementUnlocked = "AchievementUnlocked";

    public static string UserGroup(string userId) => $"user_{userId}";

    public async override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        
        await base.OnConnectedAsync();
    }
}
