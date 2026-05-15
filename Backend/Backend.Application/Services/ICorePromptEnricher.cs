using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface ICorePromptEnricher
{
    /// <summary>
    /// Обогащает промпт бизнес-контекстом: данные пользователя, цель, правила под PlanSubType.
    /// </summary>
    AIPrompt Enrich(AIPrompt prompt, GeneratePlanRequest request);
}
