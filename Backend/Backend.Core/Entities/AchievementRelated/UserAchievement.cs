namespace Backend.Core.Entities.AchievementRelated;

public class UserAchievement
{
    public Guid Id { get; set; }

    public Guid UserInfoId { get; set; }
    public UserInfo UserInfo { get; set; } = null!;

    public Guid AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;

    public DateTime AchievedAt { get; set; }
}
