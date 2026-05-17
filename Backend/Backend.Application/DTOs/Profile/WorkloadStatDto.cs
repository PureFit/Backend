namespace Backend.Application.DTOs.Profile;

public class WorkloadStatDto
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;  // "Muscle" | "BodyPart"
    public float Percent { get; set; }              // нормализованный % (считается при маппинге)
    public int SessionCount { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
