using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IPromptHelper
{
    AIPrompt Convert(GeneratePlanRequest request);
}
