namespace Backend.Application.DTOs.Excercises;

public class ExerciseTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string MeasureCategory { get; set; } = null!;
    public float Coefficient { get; set; }
    public float? ReferenceSpeedKmh { get; set; }
    public int? AverageRepsPerMinute { get; set; }
}
