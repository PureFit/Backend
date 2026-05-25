using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/social")]
public class SocialController : BaseController
{
    private readonly ISocialService _social;

    public SocialController(ISocialService social)
    {
        _social = social;
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var result = await _social.GetFriendsAsync(GetUserIdFromClaims());
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("friends/requests")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var result = await _social.GetPendingRequestsAsync(GetUserIdFromClaims());
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("friends/request/{addresseeId}")]
    public async Task<IActionResult> SendRequest(Guid addresseeId)
    {
        var result = await _social.SendFriendRequestAsync(GetUserIdFromClaims(), addresseeId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("friends/accept/{friendshipId}")]
    public async Task<IActionResult> AcceptRequest(Guid friendshipId)
    {
        var result = await _social.AcceptFriendRequestAsync(GetUserIdFromClaims(), friendshipId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("friends/decline/{friendshipId}")]
    public async Task<IActionResult> DeclineRequest(Guid friendshipId)
    {
        var result = await _social.DeclineFriendRequestAsync(GetUserIdFromClaims(), friendshipId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpDelete("friends/{targetUserId}")]
    public async Task<IActionResult> RemoveFriend(Guid targetUserId)
    {
        var result = await _social.RemoveFriendAsync(GetUserIdFromClaims(), targetUserId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("leaderboard/friends")]
    public async Task<IActionResult> FriendsLeaderboard()
    {
        var result = await _social.GetFriendsLeaderboardAsync(GetUserIdFromClaims());
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("leaderboard/global")]
    public async Task<IActionResult> GlobalLeaderboard()
    {
        var result = await _social.GetGlobalLeaderboardAsync();
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string username)
    {
        var result = await _social.SearchUsersAsync(GetUserIdFromClaims(), username ?? "");
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("profile/{targetUserId}")]
    public async Task<IActionResult> GetPublicProfile(Guid targetUserId)
    {
        var result = await _social.GetPublicProfileAsync(GetUserIdFromClaims(), targetUserId);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
