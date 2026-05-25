namespace Backend.Application.DTOs.Social;

public class FriendRequestDto
{
    public Guid FriendshipId { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterUsername { get; set; } = "";
    public string? RequesterAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
