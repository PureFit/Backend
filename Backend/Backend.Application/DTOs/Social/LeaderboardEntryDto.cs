namespace Backend.Application.DTOs.Social;

public class LeaderboardEntryDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public float TotalVolume { get; set; }
    public int Rank { get; set; }
}
