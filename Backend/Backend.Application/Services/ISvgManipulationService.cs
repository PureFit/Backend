using Backend.Application.DTOs.Muscles;

namespace Backend.Application.Services;

/// <summary>
/// Low-level SVG manipulation — works directly with raw SVG content.
/// </summary>
public interface ISvgManipulationService
{
    /// <summary>
    /// Colors each muscle element by percentage (0..100).
    /// Higher percentage = more intense color.
    /// </summary>
    string ApplyPercentages(string svgContent, IEnumerable<MusclePercentageItem> muscles);

    /// <summary>
    /// Colors target muscles and synergist muscles with distinct colors.
    /// </summary>
    string ApplyTargetSynergist(string svgContent, IEnumerable<string> targetMuscleNames, IEnumerable<string> synergistMuscleNames);
}
