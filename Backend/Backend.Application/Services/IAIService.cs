using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IAIService
{
    Task<PlanFullDto> GetPlanAsync(AIPrompt prompt);
    Task<string> ChatAsync(AIPrompt prompt);
}
