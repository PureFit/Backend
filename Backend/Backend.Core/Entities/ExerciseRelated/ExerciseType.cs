using Backend.Core.Enums;

namespace Backend.Core.Entities.ExerciseRelated;

public class ExerciseType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public MeasureCategory MeasureCategory { get; set; }

    // Коэффициент сложности внутри категории относительно базового упражнения:
    // DistanceBased: ходьба=0.4, бег=1.0, гребля=1.1
    // DurationOnly:  лёгкая йога=0.3, планка=1.0
    // RepsBased:     стретчинг=0.2, силовое=1.0
    public float Coefficient { get; set; } = 1.0f;

    // Только для DistanceBased: нормальный темп.
    // Ходьба=5, бег=10, велик=20, гребля=6
    public float? ReferenceSpeedKmh { get; set; }

    // Только для DurationOnly типа mixed упражнений: среднее кол-во репов в минуту.
    // Позволяет конвертировать duration → reps для расчёта через RepsBased формулу.
    public int? AverageRepsPerMinute { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
}
