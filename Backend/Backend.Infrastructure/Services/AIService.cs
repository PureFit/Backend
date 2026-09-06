using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly IAIClient _chatClient;
    private readonly IAIClient _planClient;
    private readonly ILogger<AIService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    public AIService(
        [FromKeyedServices("chat")] IAIClient chatClient,
        [FromKeyedServices("plan")] IAIClient planClient,
        ILogger<AIService> logger)
    {
        _chatClient = chatClient;
        _planClient = planClient;
        _logger = logger;
    }

    public async Task<AIResponse> ChatAsync(AIPrompt prompt)
    {
        _logger.LogInformation("Sending chat request to AI");
        return await _chatClient.SendAsync(prompt);
    }

    public async Task<PlanFullDto> GetPlanAsync(AIPrompt prompt)
    {
        _logger.LogInformation("Sending plan generation request to AI");

        var aiResponse = await _planClient.SendAsync(prompt);

        _logger.LogInformation("AI raw response: tokens={Tokens}, length={Length}\n{Response}",
            aiResponse.TokensUsed, aiResponse.Content.Length, aiResponse.Content);

        var json = ExtractJson(aiResponse.Content);

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
        var objStart = response.IndexOf('{');
        var arrStart = response.IndexOf('[');

        // если ответ начинается с массива — берём первый элемент
        if (arrStart != -1 && (objStart == -1 || arrStart < objStart))
        {
            var innerObj = response.IndexOf('{', arrStart);
            var arrEnd = response.LastIndexOf(']');
            if (innerObj == -1 || arrEnd == -1)
                throw new InvalidOperationException("No JSON object found inside array response");
            // берём от первого { внутри массива до последнего } перед ]
            var innerEnd = response.LastIndexOf('}', arrEnd);
            if (innerEnd == -1)
                throw new InvalidOperationException("No closing brace found inside array response");
            return response[innerObj..(innerEnd + 1)];
        }

        if (objStart == -1)
            throw new InvalidOperationException("No JSON found in AI response");
        var end = response.LastIndexOf('}');
        return response[objStart..(end + 1)];
    }
}
