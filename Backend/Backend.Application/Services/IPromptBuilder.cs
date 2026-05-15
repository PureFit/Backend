using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IPromptBuilder
{
    /// <summary>
    /// Строит базовый промпт: структура + JSON схема ответа + каталог упражнений.
    /// </summary>
    Task<AIPrompt> BuildAsync(GeneratePlanRequest request);
}
