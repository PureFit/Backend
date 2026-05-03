namespace Backend.Application.DTOs.Excercises;

public class ExercisePagedResult
{
    public List<ExerciseDto> Items { get; set; } = new();
    public bool HasNextPage { get; set; }
    public string? NextCursor { get; set; }
    public int Total { get; set; }
}
