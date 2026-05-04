namespace Backend.Application.DTOs.Excercises;

public class ExerciseFilter
{
    public string? Keywords { get; set; }
    public string? BodyPart { get; set; }
    public string? Muscle { get; set; }
    public string? Equipment { get; set; }
    public string? Type { get; set; }
    public string? Cursor { get; set; }
    public int Limit { get; set; } = 20;
}
