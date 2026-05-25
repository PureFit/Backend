namespace Backend.Application.DTOs.Social;

public class SentFriendRequestDto
{
    public Guid FriendshipId { get; set; }
    public Guid AddresseeId { get; set; }
    public string AddresseeUsername { get; set; } = "";
    public string? AddresseeAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
