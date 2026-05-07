namespace Backend.Application.DTOs.TrainingSet;

public class TrainingSetPagedResult
{
    public List<TrainingSetResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
