using Backend.Application.DTOs.Muscles;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/muscles/svg")]
public class MuscleVisualizationController : BaseController
{
    private readonly IMuscleVisualizationService _visualizationService;

    public MuscleVisualizationController(IMuscleVisualizationService visualizationService)
    {
        _visualizationService = visualizationService;
    }

    /// <summary>
    /// Returns SVG with muscles colored by activation percentage (0..100 per muscle).
    /// </summary>
    [HttpPost("percentage")]
    public async Task<IActionResult> GetPercentageSvg([FromBody] MuscleSvgPercentageRequest request)
    {
        var svg = await _visualizationService.GetPercentageSvgAsync(request);
        return Content(svg, "image/svg+xml");
    }

    /// <summary>
    /// Returns SVG with target muscles and synergist muscles highlighted in distinct colors.
    /// </summary>
    [HttpPost("target")]
    public async Task<IActionResult> GetTargetSvg([FromBody] MuscleSvgTargetRequest request)
    {
        var svg = await _visualizationService.GetTargetSynergistSvgAsync(request);
        return Content(svg, "image/svg+xml");
    }
}
