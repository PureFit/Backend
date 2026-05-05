using Backend.Core.Entities;

namespace Backend.Application.DTOs.Profile;

public class CompleteProfileRequest
{
    public Sex Sex { get; set; }
    public Level FitnessLevel { get; set; }
    public decimal WeightKg { get; set; }
    public int HeightCm { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
