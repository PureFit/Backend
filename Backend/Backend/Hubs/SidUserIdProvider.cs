using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Backend.Hubs;

public class SidUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
}
