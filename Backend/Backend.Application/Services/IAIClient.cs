using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IAIClient
{
    Task<AIResponse> SendAsync(AIPrompt prompt);
}
