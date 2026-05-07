using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class CreateSetRequest
{
    public Guid UserId { get; set; } // set from JWT claims in controller
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public SetAccessType SetAccessType { get; set; }
    public TrainingType? TrainingType { get; set; }
    public BodyPartFocus? BodyPartFocus { get; set; }
}
