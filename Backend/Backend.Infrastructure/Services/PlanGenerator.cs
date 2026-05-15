using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Services;

public class PlanGenerator : IPlanGenerator
{
    private readonly IPromptBuilder _promptBuilder;
    private readonly IAIService _aiService;
    private readonly ICorePromptEnricher _corePromptEnricher;
    private readonly ILogger<PlanGenerator> _logger;

    public PlanGenerator(
        IPromptBuilder promptBuilder,
        IAIService aiService,
        ICorePromptEnricher corePromptEnricher,
        ILogger<PlanGenerator> logger)
    {
        _promptBuilder = promptBuilder;
        _aiService = aiService;
        _corePromptEnricher = corePromptEnricher;
        _logger = logger;
    }

    public async Task<BaseResponse<PlanFullDto>> GeneratePlanAsync(GeneratePlanRequest request)
    {
        try
        {
            var prompt = await _promptBuilder.BuildAsync(request);
            var enrichedPrompt = _corePromptEnricher.Enrich(prompt, request);
            var plan = await _aiService.GetPlanAsync(enrichedPrompt);
            return BaseResponse<PlanFullDto>.Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Plan generation failed. Type: {PlanType}, SubType: {PlanSubType}",
                request.PlanType, request.PlanSubType);
            return BaseResponse<PlanFullDto>.Fail(ErrorEnums.PlanGenerationFailed);
        }
    }
}
