namespace Backend.Application.DTOs.TrainingSet;

public class SetBlockResponse
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public int SetsCount { get; set; }
    public int RestBetweenSetsSeconds { get; set; }
    public int RestAfterBlockSeconds { get; set; }
    public List<ExerciseEntryResponse> ExerciseEntries { get; set; } = [];
}
