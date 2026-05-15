using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IAIService
{
    /// <summary>
    /// Отправляет промпт в Claude, возвращает распарсенный план.
    /// </summary>
    Task<PlanFullDto> GetPlanAsync(AIPrompt prompt);
}
