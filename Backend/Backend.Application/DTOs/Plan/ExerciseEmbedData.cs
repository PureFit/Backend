namespace Backend.Application.DTOs.Plan;

public class ExerciseEmbedData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public List<string> Muscles { get; set; } = [];
    public List<string> BodyParts { get; set; } = [];
    public List<string> Equipment { get; set; } = [];
    public List<string> Keywords { get; set; } = [];
}
