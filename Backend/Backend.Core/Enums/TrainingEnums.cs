namespace Backend.Core.Enums;

public enum PlanStatus
{
    Active,
    Completed,
    Paused,
    Archived
}

public enum PlanType
{
    Performance,
    Mix,
    BodyComposition,
    AestheticShaping,
    HealthAndRehabilitation
}

public enum WeekStatus
{
    Future,
    Active,
    Completed,
    Skipped
}

public enum SetAccessType
{
    Private,
    Public
}

public enum MeasureType
{
    Reps,
    DurationSeconds,
    DistanceMeters
}

public enum WorkloadStatCategory
{
    Muscle,
    BodyPart
}

public enum SessionStatus
{
    InProgress,
    Completed,
    Abandoned
}

public enum TrainingType
{
    FullBody,
    Push,
    Pull,
    Strength,
    Hypertrophy,
    Cardio,
    Mobility
}

public enum BodyPartFocus
{
    Chest,
    Back,
    Legs,
    Shoulders,
    Arms,
    Core
}

// RPE (Rate of Perceived Exertion) — субъективная оценка нагрузки от пользователя.
public enum PerceivedExertion
{
    VeryLight = 1,   // разминка, прогулка
    Light = 3,       // легко, можно говорить
    Moderate = 5,    // средне, дыхание учащённое
    Hard = 7,        // тяжело, говорить трудно
    VeryHard = 9,    // почти максимум
    Maximum = 10     // полная выкладка
}
