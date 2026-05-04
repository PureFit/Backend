namespace Backend.Application.DTOs.Muscles;

public class MusclePercentageItem
{
    public string MuscleName { get; set; } = null!;
    public float Percentage { get; set; } // 0..100
}
