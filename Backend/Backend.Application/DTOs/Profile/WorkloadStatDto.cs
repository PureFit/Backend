namespace Backend.Application.DTOs.Profile;

public class WorkloadStatDto
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!; // "Muscle" | "BodyPart"
    public float TotalPercent { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
