namespace Backend.Application.Common;

public class ExerciseParameterConfig
{
    // Базовый коэффициент за 1 реп (без веса).
    // Масштабирует репы к общей системе единиц.
    public float RepCoefficient { get; set; } = 1.0f;

    // Базовый коэффициент за 1 метр дистанции.
    // Настраивается чтобы 1км бега ≈ разумному кол-ву репов.
    public float DistanceCoefficient { get; set; } = 0.03f;

    // Базовый коэффициент за 1 секунду (для DurationOnly упражнений без движения).
    public float DurationCoefficient { get; set; } = 0.033f;
}
