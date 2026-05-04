namespace Backend.Application.Common;

public static class CacheKeys
{
    public static string Exercises(string? keywords, string? bodyPart, string? muscle,
                                   string? equipment, string? type, string? cursor, int limit)
        => $"exercises:{keywords}:{bodyPart}:{muscle}:{equipment}:{type}:{cursor}:{limit}";

    public static string ExerciseDetail(string id) => $"exercise:{id}";

    public const string Muscles       = "muscles";
    public const string BodyParts     = "body-parts";
    public const string Equipments    = "equipments";
    public const string ExerciseTypes = "exercise-types";
}
