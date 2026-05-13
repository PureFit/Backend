namespace Backend.Application.DTOs.TrainingSet;

public class TrainingSetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SetAccessType { get; set; } = null!;
    public string? TrainingType { get; set; }
    public string? BodyPartFocus { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Dictionary<string, float>? MusclePercentages { get; set; }
    public Dictionary<string, float>? BodyPartPercentages { get; set; }
    public int TotalSessionsCount { get; set; }
    public int UniqueUsersCount { get; set; }
    public List<SetBlockResponse> SetBlocks { get; set; } = [];
}
