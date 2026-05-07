namespace Backend.Application.Common;

public class ExerciseParameterConfig
{
    public float SecondsPerRepEquivalent { get; set; } = 30f;
    public float RepCoefficient { get; set; } = 1f;
    public float MeterCoefficient { get; set; } = 0.03f;
    public List<ParameterEntry> Parameters { get; set; } = [];
}

public class ParameterEntry
{
    public string Name { get; set; } = null!;
    public ParameterRole Role { get; set; }
    public float Factor { get; set; }
}

public enum ParameterRole
{
    RepMultiplier,
    DurationIntensity,
    AdditiveModifier,
    SpeedKmh
}
