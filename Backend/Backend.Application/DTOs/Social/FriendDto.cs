namespace Backend.Application.DTOs.Social;

public class FriendDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string? AvatarUrl { get; set; }
}
