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

    public async Task<PlanFullDto> GetPlanAsync(AIPrompt prompt)
    {
        _logger.LogInformation("Sending plan generation request to AI");

        var response = await _aiClient.SendAsync(prompt);

        _logger.LogDebug("AI raw response length: {Length} chars", response.Length);

        var json = ExtractJson(response);

        return JsonSerializer.Deserialize<PlanFullDto>(json, _jsonOptions) ?? throw new InvalidOperationException("AI returned empty plan");
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
