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
    private readonly ICacheService _cacheService;

    public MuscleVisualizationService(
        ISvgManipulationService svgManipulationService,
        IHostEnvironment env,
        IOptions<SvgSettings> svgSettings,
        ICacheService cacheService)
    {
        _svgManipulationService = svgManipulationService;
        _env = env;
        _svgSettings = svgSettings.Value;
        _cacheService = cacheService;
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
        var cacheKey = CacheKeys.SvgBase(isMale);
        var cached = await _cacheService.GetAsync<string>(cacheKey);
        if (cached is not null) return cached;

        var fileName = isMale ? _svgSettings.ManSvgFileName : _svgSettings.WomanSvgFileName;
        var path = Path.Combine(_env.ContentRootPath, fileName);
        var content = await File.ReadAllTextAsync(path);

        await _cacheService.SetAsync(cacheKey, content, TimeSpan.FromDays(30));
        return content;
    }
}
