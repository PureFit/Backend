namespace Backend.Application.DTOs.Excercises;

public class ExerciseFilter
{
    public string? Keywords { get; set; }
    public string? BodyPart { get; set; }
    public string? Muscle { get; set; }
    public string? Equipment { get; set; }
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
