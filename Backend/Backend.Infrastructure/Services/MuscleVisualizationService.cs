using Backend.Application.Common;
using Backend.Application.DTOs.Muscles;
using Backend.Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Services;

public class MuscleVisualizationService : IMuscleVisualizationService
{
    private readonly ISvgManipulationService _svgManipulationService;
    private readonly IHostEnvironment _env;
    private readonly SvgSettings _svgSettings;

    public MuscleVisualizationService(
        ISvgManipulationService svgManipulationService,
        IHostEnvironment env,
        IOptions<SvgSettings> svgSettings)
    {
        _svgManipulationService = svgManipulationService;
        _env = env;
        _svgSettings = svgSettings.Value;
    }

    public async Task<string> GetPercentageSvgAsync(MuscleSvgPercentageRequest request)
    {
        var svgContent = await ReadSvgAsync(request.IsMale);
        return _svgManipulationService.ApplyPercentages(svgContent, request.Muscles);
    }

    public async Task<string> GetTargetSynergistSvgAsync(MuscleSvgTargetRequest request)
    {
        var svgContent = await ReadSvgAsync(request.IsMale);
        return _svgManipulationService.ApplyTargetSynergist(svgContent, request.TargetMuscles, request.SynergistMuscles);
    }

    private async Task<string> ReadSvgAsync(bool isMale)
    {
        var fileName = isMale ? _svgSettings.ManSvgFileName : _svgSettings.WomanSvgFileName;
        var path = Path.Combine(_env.ContentRootPath, fileName);
        return await File.ReadAllTextAsync(path);
    }
}
