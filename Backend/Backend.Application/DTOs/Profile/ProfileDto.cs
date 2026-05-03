using Backend.Core.Entities;

namespace Backend.Application.DTOs.Profile;

public class ProfileDto
{
    public string Username { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Sex { get; set; } = null!;
    public string FitnessLevel { get; set; } = null!;
    public decimal WeightKg { get; set; }
    public int HeightCm { get; set; }
    public int Age { get; set; }
}
