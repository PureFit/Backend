using Backend.Application.DTOs.Achievement;
using Backend.Application.DTOs.Profile;
using Backend.Application.DTOs.TrainingSet;

namespace Backend.Application.DTOs.Social;

public class UserPublicProfileDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public List<TrainingSetResponse> PublicSets { get; set; } = [];
    public List<AchievementDto> Achievements { get; set; } = [];
    public List<WorkloadStatDto> WorkloadStats { get; set; } = [];
    public string? FriendshipStatus { get; set; }
    public bool IsRequester { get; set; }
    public Guid? FriendshipId { get; set; }
}
