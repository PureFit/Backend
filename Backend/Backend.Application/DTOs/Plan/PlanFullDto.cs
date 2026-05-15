namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Структура плана возвращаемая AI — без дат, только контент.
/// Даты проставляет PlanScheduler отдельно.
/// </summary>
public class PlanFullDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<WeekFullDto> Weeks { get; set; } = [];
}

public class WeekFullDto
{
    public int WeekNumber { get; set; }
    public string Description { get; set; } = null!;
    public List<TrainingFullDto> Trainings { get; set; } = [];
}

public class TrainingFullDto
{
    public int TrainingNumber { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int EstimatedDurationMinutes { get; set; }
    public List<SetBlockFullDto> Blocks { get; set; } = [];
}

/// <summary>
/// Соответствует SetBlock — группа упражнений выполняемая N раз подряд.
/// </summary>
public class SetBlockFullDto
{
    public int Order { get; set; }
    public string? Name { get; set; }
    public int SetsCount { get; set; }                  // сколько раз повторить блок
    public int RestBetweenSetsSeconds { get; set; }     // отдых между повторениями блока
    public int RestAfterBlockSeconds { get; set; }      // отдых после всего блока
    public List<ExerciseEntryFullDto> Exercises { get; set; } = [];
}

/// <summary>
/// Соответствует ExerciseEntry — одно упражнение внутри блока.
/// </summary>
public class ExerciseEntryFullDto
{
    public int Order { get; set; }
    public Guid ExerciseId { get; set; }

    // Базовые параметры (заполняется если нет интервалов)
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }

    public int RestAfterCurrentEntrySeconds { get; set; }

    // Интервалы (заполняется если упражнение интервальное — например HIIT, пирамида)
    public List<ExerciseIntervalFullDto> Intervals { get; set; } = [];

    public string? Notes { get; set; }
}

/// <summary>
/// Соответствует ExerciseInterval — один интервал внутри упражнения.
/// </summary>
public class ExerciseIntervalFullDto
{
    public int Order { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public float? DistanceMeters { get; set; }
    public float? WeightKg { get; set; }
    public float? SpeedKmh { get; set; }
}
