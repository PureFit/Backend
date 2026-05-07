namespace Backend.Application.DTOs.TrainingSet;

public class UpdateSetBlockRequest
{
    public Guid SetId { get; set; }
    public Guid BlockId { get; set; }
    public int? SetsCount { get; set; }
    public int? RestBetweenSetsSeconds { get; set; }
    public int? RestAfterBlockSeconds { get; set; }
    public int? Order { get; set; }
}
