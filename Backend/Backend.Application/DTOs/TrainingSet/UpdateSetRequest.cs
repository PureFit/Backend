using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class UpdateSetRequest
{
    public Guid SetId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public SetAccessType? SetAccessType { get; set; }
    public TrainingType? TrainingType { get; set; }
    public BodyPartFocus? BodyPartFocus { get; set; }
}
