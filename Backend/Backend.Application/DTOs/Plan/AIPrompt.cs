namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Промпт готовый к отправке в AI — system message + user message.
/// </summary>
public class AIPrompt
{
    public string SystemMessage { get; set; } = null!;
    public string UserMessage { get; set; } = null!;

    // index (1,2,3...) → real exercise UUID, populated when building the catalog
    public Dictionary<int, Guid> ExerciseIndexMap { get; set; } = [];
}

