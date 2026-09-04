using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly IAIClient _aiClient;
    private readonly ILogger<AIService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    public AIService(IAIClient aiClient, ILogger<AIService> logger)
    {
        _aiClient = aiClient;
        _logger = logger;
    }

    public async Task<string> ChatAsync(AIPrompt prompt)
    {
        _logger.LogInformation("Sending chat request to AI");
        return await _aiClient.SendAsync(prompt);
    }

    public async Task<PlanFullDto> GetPlanAsync(AIPrompt prompt)
    {
        _logger.LogInformation("Sending plan generation request to AI");

        var response = await _aiClient.SendAsync(prompt);

        _logger.LogDebug("AI raw response length: {Length} chars", response.Length);

        var json = ExtractJson(response);

        if (prompt.ExerciseIndexMap.Count > 0)
            json = ResolveExerciseIndices(json, prompt.ExerciseIndexMap);

        return JsonSerializer.Deserialize<PlanFullDto>(json, _jsonOptions) ?? throw new InvalidOperationException("AI returned empty plan");
    }

    // AI returns exerciseId as an integer index (1, 2, 3...) — map back to real UUIDs.
    // Process in descending order to avoid replacing "1" inside "10", "11", etc.
    private static string ResolveExerciseIndices(string json, Dictionary<int, Guid> map)
    {
        foreach (var (idx, guid) in map.OrderByDescending(x => x.Key))
        {
            var uuidJson = $"\"exerciseId\":\"{guid}\"";
            // number form (with and without space after colon)
            json = json.Replace($"\"exerciseId\":{idx}", uuidJson);
            json = json.Replace($"\"exerciseId\": {idx}", uuidJson);
            // string form (AI sometimes wraps in quotes)
            json = json.Replace($"\"exerciseId\":\"{idx}\"", uuidJson);
            json = json.Replace($"\"exerciseId\": \"{idx}\"", uuidJson);
        }
        return json;
    }

    private static string ExtractJson(string response)
    {
        var start = response.IndexOf('{');
        var end = response.LastIndexOf('}');
        if (start == -1 || end == -1)
            throw new InvalidOperationException("No JSON found in AI response");
        return response[start..(end + 1)];
    }
}
