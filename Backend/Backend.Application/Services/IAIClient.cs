using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IAIClient
{
    Task<string> SendAsync(AIPrompt prompt);
}
