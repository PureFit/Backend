namespace Backend.Application.DTOs.Excercises;

public class ExercisePagedResult
{
    public List<ExerciseDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
