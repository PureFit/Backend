namespace Backend.Application.Common;

/// <summary>
/// Ключи конфига в таблице AppSettings.
/// Значения меняются через AdminController без передеплоя.
/// </summary>
public static class AppSettingKeys
{
    /// <summary>Включён ли AI-чат глобально (true/false)</summary>
    public const string ChatEnabled = "ai:chat:enabled";

    /// <summary>Лимит токенов на пользователя в день. 0 = безлимит.</summary>
    public const string DailyTokenLimit = "ai:chat:daily_token_limit";

    /// <summary>Лимит запросов на пользователя в день. 0 = безлимит.</summary>
    public const string DailyRequestLimit = "ai:chat:daily_request_limit";
}
