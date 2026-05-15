namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Промпт готовый к отправке в AI — system message + user message.
/// </summary>
public class AIPrompt
{
    public string SystemMessage { get; set; } = null!;
    public string UserMessage { get; set; } = null!;
}

