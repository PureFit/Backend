using Backend.Application.Common;
using Backend.Application.DTOs.Achievement;
using Backend.Application.DTOs.Profile;
using Backend.Application.DTOs.Social;
using Backend.Application.Repositories;
using Backend.Application.Services;
using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class SocialService : ISocialService
{
    private readonly IFriendshipRepository _friendshipRepo;
    private readonly IAuthRepository _authRepo;
    private readonly IUserInfoRepository _userInfoRepo;
    private readonly IAchievementRepository _achievementRepo;
    private readonly ITrainingSetService _setService;
    private readonly ILogger<SocialService> _logger;

    public SocialService(
        IFriendshipRepository friendshipRepo,
        IAuthRepository authRepo,
        IUserInfoRepository userInfoRepo,
        IAchievementRepository achievementRepo,
        ITrainingSetService setService,
        ILogger<SocialService> logger)
    {
        _friendshipRepo = friendshipRepo;
        _authRepo = authRepo;
        _userInfoRepo = userInfoRepo;
        _achievementRepo = achievementRepo;
        _setService = setService;
        _logger = logger;
    }

    public async Task<BaseResponse<List<FriendDto>>> GetFriendsAsync(Guid userId)
    {
        try
        {
            var friendships = await _friendshipRepo.GetFriendsAsync(userId);
            var result = friendships.Select(f =>
            {
                var friend = f.RequesterId == userId ? f.Addressee : f.Requester;
                return new FriendDto { UserId = friend.Id, Username = friend.Username, AvatarUrl = friend.AvatarUrl };
            }).ToList();
            return BaseResponse<List<FriendDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFriendsAsync failed for {UserId}", userId);
            return BaseResponse<List<FriendDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<FriendRequestDto>>> GetPendingRequestsAsync(Guid userId)
    {
        try
        {
            var requests = await _friendshipRepo.GetPendingRequestsAsync(userId);
            var result = requests.Select(f => new FriendRequestDto
            {
                FriendshipId = f.Id,
                RequesterId = f.RequesterId,
                RequesterUsername = f.Requester.Username,
                RequesterAvatarUrl = f.Requester.AvatarUrl,
                CreatedAt = f.CreatedAt
            }).ToList();
            return BaseResponse<List<FriendRequestDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPendingRequestsAsync failed for {UserId}", userId);
            return BaseResponse<List<FriendRequestDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> SendFriendRequestAsync(Guid requesterId, Guid addresseeId)
    {
        try
        {
            if (requesterId == addresseeId)
                return BaseResponse<bool>.Fail(ErrorEnums.ValidationError);

            var existing = await _friendshipRepo.GetAsync(requesterId, addresseeId);
            if (existing is not null)
                return BaseResponse<bool>.Fail(ErrorEnums.ValidationError);

            var addressee = await _authRepo.GetByIdAsync(addresseeId);
            if (addressee is null)
                return BaseResponse<bool>.Fail(ErrorEnums.UserNotFound);

            await _friendshipRepo.AddAsync(new UserFriendship
            {
                Id = Guid.NewGuid(),
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendFriendRequestAsync failed");
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> AcceptFriendRequestAsync(Guid userId, Guid friendshipId)
    {
        try
        {
            var friendship = await _friendshipRepo.GetByIdAsync(friendshipId);
            if (friendship is null || friendship.AddresseeId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            if (friendship.Status != FriendshipStatus.Pending)
                return BaseResponse<bool>.Fail(ErrorEnums.ValidationError);

            friendship.Status = FriendshipStatus.Accepted;
            await _friendshipRepo.UpdateAsync(friendship);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AcceptFriendRequestAsync failed for {FriendshipId}", friendshipId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> DeclineFriendRequestAsync(Guid userId, Guid friendshipId)
    {
        try
        {
            var friendship = await _friendshipRepo.GetByIdAsync(friendshipId);
            if (friendship is null || friendship.AddresseeId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            await _friendshipRepo.DeleteAsync(friendship);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeclineFriendRequestAsync failed");
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> RemoveFriendAsync(Guid userId, Guid targetUserId)
    {
        try
        {
            var friendship = await _friendshipRepo.GetAsync(userId, targetUserId);
            if (friendship is null || friendship.Status != FriendshipStatus.Accepted)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            await _friendshipRepo.DeleteAsync(friendship);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveFriendAsync failed");
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<LeaderboardEntryDto>>> GetFriendsLeaderboardAsync(Guid userId)
    {
        try
        {
            var board = await _friendshipRepo.GetFriendsLeaderboardAsync(userId);
            return BaseResponse<List<LeaderboardEntryDto>>.Ok(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFriendsLeaderboardAsync failed for {UserId}", userId);
            return BaseResponse<List<LeaderboardEntryDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<LeaderboardEntryDto>>> GetGlobalLeaderboardAsync()
    {
        try
        {
            var board = await _friendshipRepo.GetGlobalLeaderboardAsync(50);
            return BaseResponse<List<LeaderboardEntryDto>>.Ok(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGlobalLeaderboardAsync failed");
            return BaseResponse<List<LeaderboardEntryDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<UserSearchResultDto>>> SearchUsersAsync(Guid currentUserId, string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
                return BaseResponse<List<UserSearchResultDto>>.Ok([]);

            var users = await _authRepo.SearchByUsernameAsync(username.Trim());

            var result = new List<UserSearchResultDto>();
            foreach (var user in users)
            {
                if (user.Id == currentUserId) continue;

                var friendship = await _friendshipRepo.GetAsync(currentUserId, user.Id);
                result.Add(new UserSearchResultDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    AvatarUrl = user.AvatarUrl,
                    FriendshipStatus = friendship?.Status.ToString(),
                    IsRequester = friendship?.RequesterId == currentUserId,
                    FriendshipId = friendship?.Id
                });
            }

            return BaseResponse<List<UserSearchResultDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchUsersAsync failed");
            return BaseResponse<List<UserSearchResultDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<UserPublicProfileDto>> GetPublicProfileAsync(Guid currentUserId, Guid targetUserId)
    {
        try
        {
            var user = await _authRepo.GetByIdAsync(targetUserId);
            if (user is null)
                return BaseResponse<UserPublicProfileDto>.Fail(ErrorEnums.UserNotFound);

            var userInfo = await _userInfoRepo.GetByUserIdWithStatsAsync(targetUserId);

            var achievements = new List<AchievementDto>();
            var stats = new List<WorkloadStatDto>();

            if (userInfo is not null)
            {
                var allAchievements = await _achievementRepo.GetAllAsync();
                var unlocked = await _achievementRepo.GetByUserInfoIdAsync(userInfo.Id);
                var unlockedMap = unlocked.ToDictionary(ua => ua.AchievementId, ua => ua.AchievedAt);

                achievements = allAchievements.Select(a => new AchievementDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IconEmoji = a.IconEmoji,
                    IsUnlocked = unlockedMap.ContainsKey(a.Id),
                    AchievedAt = unlockedMap.TryGetValue(a.Id, out var dt) ? dt : null
                }).ToList();

                var muscleTotal   = userInfo.WorkloadStats.Where(s => s.Category == WorkloadStatCategory.Muscle).Sum(s => s.AccumulatedVolume);
                var bodyPartTotal = userInfo.WorkloadStats.Where(s => s.Category == WorkloadStatCategory.BodyPart).Sum(s => s.AccumulatedVolume);

                stats = userInfo.WorkloadStats.Select(s =>
                {
                    var total = s.Category == WorkloadStatCategory.Muscle ? muscleTotal : bodyPartTotal;
                    return new WorkloadStatDto
                    {
                        Name = s.Name,
                        Category = s.Category.ToString(),
                        Percent = total > 0 ? s.AccumulatedVolume / total * 100f : 0f,
                        RawVolume = s.AccumulatedVolume,
                        SessionCount = s.SessionCount,
                        LastUpdatedAt = s.LastUpdatedAt
                    };
                }).ToList();
            }

            var setsResult = await _setService.GetPublicSetsByUserAsync(targetUserId);
            var publicSets = setsResult.Success ? setsResult.Data ?? [] : [];

            var friendship = await _friendshipRepo.GetAsync(currentUserId, targetUserId);

            return BaseResponse<UserPublicProfileDto>.Ok(new UserPublicProfileDto
            {
                UserId = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                PublicSets = publicSets,
                Achievements = achievements,
                WorkloadStats = stats,
                FriendshipStatus = friendship?.Status.ToString(),
                IsRequester = friendship?.RequesterId == currentUserId,
                FriendshipId = friendship?.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPublicProfileAsync failed for {TargetUserId}", targetUserId);
            return BaseResponse<UserPublicProfileDto>.Fail(ErrorEnums.UnknownError);
        }
    }
}
