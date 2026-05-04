namespace Backend.Application.DTOs.Muscles;

public class MuscleSvgPercentageRequest
{
    public bool IsMale { get; set; } = true;
    public List<MusclePercentageItem> Muscles { get; set; } = [];
}
