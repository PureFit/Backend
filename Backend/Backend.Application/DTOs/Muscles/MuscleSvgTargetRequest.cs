namespace Backend.Application.DTOs.Muscles;

public class MuscleSvgTargetRequest
{
    public bool IsMale { get; set; } = true;
    public List<string> TargetMuscles { get; set; } = [];
    public List<string> SynergistMuscles { get; set; } = [];
}
