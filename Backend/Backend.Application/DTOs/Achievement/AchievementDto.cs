namespace Backend.Application.DTOs.Achievement;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? IconEmoji { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? AchievedAt { get; set; }
}
