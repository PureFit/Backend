using Backend.Application.DTOs.Plan;
using Backend.Application.Helpers;
using Backend.Application.Repositories;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class PromptBuilder : IPromptBuilder
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IEmbeddingClient _embeddingClient;

    public PromptBuilder(IExerciseRepository exerciseRepository, IEmbeddingClient embeddingClient)
    {
        _exerciseRepository = exerciseRepository;
        _embeddingClient = embeddingClient;
    }

    public async Task<AIPrompt> BuildAsync(GeneratePlanRequest request)
    {
        var queryText = BuildQueryText(request);
        var queryVec = await _embeddingClient.EmbedAsync(queryText);

        var exercises = await _exerciseRepository.SearchByEmbeddingAsync(
            queryVec,
            request.AvailableEquipment,
            topN: 60);

        return PromptHelper
            .CreateBase()
            .AddUserRequest(request)
            .AddExercises(exercises);
    }

    private static string BuildQueryText(GeneratePlanRequest request)
    {
        var parts = new List<string> { request.PlanSubType };

        if (!string.IsNullOrWhiteSpace(request.FitnessLevel))
            parts.Add($"fitness level: {request.FitnessLevel}");
        if (request.AvailableEquipment.Count > 0)
            parts.Add($"equipment: {string.Join(", ", request.AvailableEquipment)}");
        if (!string.IsNullOrWhiteSpace(request.GoalMetadata))
            parts.Add(request.GoalMetadata);
        if (!string.IsNullOrWhiteSpace(request.FreeTextWish))
            parts.Add(request.FreeTextWish);

        return string.Join(" ", parts);
    }
}
