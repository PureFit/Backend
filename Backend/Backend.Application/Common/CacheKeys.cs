using Backend.Application.DTOs.Excercises;

namespace Backend.Application.Common;

public static class CacheKeys
{
    public static string Exercises(ExerciseFilter exerciseFilter)
    {
        var parts = new (string Key, string? Value)[]
        {
            ("kw",   exerciseFilter.Keywords),
            ("bp",   exerciseFilter.BodyPart),
            ("m",    exerciseFilter.Muscle),
            ("eq",   exerciseFilter.Equipment),
            ("t",    exerciseFilter.Category),
            ("p",    exerciseFilter.Page.ToString()),
            ("ps",   exerciseFilter.PageSize.ToString()),
        };

        var segment = string.Join("|", parts
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .Select(p => $"{p.Key}={p.Value!.Trim().ToLowerInvariant()}"));

        return string.IsNullOrEmpty(segment) ? "exercises:all" : $"exercises:{segment}";
    }

    public static string ExerciseDetail(string id) => $"exercise:{id}";
    public const string ExercisesBrief = $"exercises:brief";

    public const string Muscles       = "muscles";
    public const string BodyParts     = "body-parts";
    public const string Equipments    = "equipments";
    public const string ExerciseTypes = "exercise-types";

    public static string SvgBase(bool isMale) => isMale ? "svg:base:male" : "svg:base:female";

    public static string ChatContext(Guid userId) => $"chat:context:{userId}";
    public static string ChatHistory(Guid userId) => $"chat:history:{userId}";

    public static string AppSetting(string key)         => $"settings:{key}";
    public static string DailyTokens(Guid userId, string date)   => $"ai:usage:tokens:{userId}:{date}";
    public static string DailyRequests(Guid userId, string date) => $"ai:usage:requests:{userId}:{date}";
}
