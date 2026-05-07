namespace Backend.Application.DTOs.TrainingSet;

public class AddSetBlockRequest
{
    public Guid SetId { get; set; }
    public Guid UserId { get; set; } // set from JWT claims in controller
    public int SetsCount { get; set; } = 1;
    public int RestBetweenSetsSeconds { get; set; }
    public int RestAfterBlockSeconds { get; set; }
}
