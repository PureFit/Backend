using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IAIService
{
    Task<PlanFullDto> GetPlanAsync(AIPrompt prompt);
    Task<AIResponse> ChatAsync(AIPrompt prompt);
}
