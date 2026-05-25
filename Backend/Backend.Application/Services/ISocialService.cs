using Backend.Application.Common;
using Backend.Application.DTOs.Social;

namespace Backend.Application.Services;

public interface ISocialService
{
    Task<BaseResponse<List<FriendDto>>> GetFriendsAsync(Guid userId);
    Task<BaseResponse<List<FriendRequestDto>>> GetPendingRequestsAsync(Guid userId);
    Task<BaseResponse<bool>> SendFriendRequestAsync(Guid requesterId, Guid addresseeId);
    Task<BaseResponse<bool>> AcceptFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<BaseResponse<bool>> DeclineFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<BaseResponse<bool>> RemoveFriendAsync(Guid userId, Guid targetUserId);
    Task<BaseResponse<List<LeaderboardEntryDto>>> GetFriendsLeaderboardAsync(Guid userId);
    Task<BaseResponse<List<LeaderboardEntryDto>>> GetGlobalLeaderboardAsync();
    Task<BaseResponse<List<UserSearchResultDto>>> SearchUsersAsync(Guid currentUserId, string username);
    Task<BaseResponse<UserPublicProfileDto>> GetPublicProfileAsync(Guid currentUserId, Guid targetUserId);
}
