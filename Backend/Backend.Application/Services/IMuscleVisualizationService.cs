using Backend.Application.DTOs.Muscles;

namespace Backend.Application.Services;

public interface IMuscleVisualizationService
{
    /// <summary>
    /// Returns an SVG with muscles colored by their activation percentage.
    /// </summary>
    Task<string> GetPercentageSvgAsync(MuscleSvgPercentageRequest request);

    /// <summary>
    /// Returns an SVG with target muscles and synergist muscles highlighted in distinct colors.
    /// </summary>
    Task<string> GetTargetSynergistSvgAsync(MuscleSvgTargetRequest request);
}
