namespace Backend.Application.DTOs.Chat;

public class UserUsageDto
{
    public int TokensUsedToday { get; set; }
    public int RequestsToday { get; set; }
    public int DailyTokenLimit { get; set; }
    public int DailyRequestLimit { get; set; }

    /// <summary>0 означает безлимит.</summary>
    public bool IsTokenLimitReached => DailyTokenLimit > 0 && TokensUsedToday >= DailyTokenLimit;
    public bool IsRequestLimitReached => DailyRequestLimit > 0 && RequestsToday >= DailyRequestLimit;
}
