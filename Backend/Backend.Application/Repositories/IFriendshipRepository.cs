using Backend.Application.DTOs.Social;
using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IFriendshipRepository
{
    Task<UserFriendship?> GetAsync(Guid userId1, Guid userId2);
    Task<UserFriendship?> GetByIdAsync(Guid friendshipId);
    Task<List<UserFriendship>> GetFriendsAsync(Guid userId);
    Task<List<UserFriendship>> GetPendingRequestsAsync(Guid addresseeId);
    Task AddAsync(UserFriendship friendship);
    Task UpdateAsync(UserFriendship friendship);
    Task DeleteAsync(UserFriendship friendship);

    Task<List<LeaderboardEntryDto>> GetFriendsLeaderboardAsync(Guid userId);
    Task<List<LeaderboardEntryDto>> GetGlobalLeaderboardAsync(int top = 50);
}
