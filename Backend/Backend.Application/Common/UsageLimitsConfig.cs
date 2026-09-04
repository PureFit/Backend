namespace Backend.Application.Common;

public class UsageLimitsConfig
{
    /// <summary>Макс токенов на юзера в день. 0 = безлимит.</summary>
    public int DailyTokenLimit { get; set; } = 0;

    /// <summary>Макс запросов на юзера в день. 0 = безлимит.</summary>
    public int DailyRequestLimit { get; set; } = 0;

    /// <summary>Глобальный выключатель AI-чата.</summary>
    public bool ChatEnabled { get; set; } = true;
}
