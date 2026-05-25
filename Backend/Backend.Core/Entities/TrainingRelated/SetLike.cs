using Backend.Core.Entities;

namespace Backend.Core.Entities.TrainingRelated;

public class SetLike
{
    public Guid Id { get; set; }
    public Guid TrainingSetId { get; set; }
    public TrainingSet TrainingSet { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsLike { get; set; }
    public DateTime CreatedAt { get; set; }
}
