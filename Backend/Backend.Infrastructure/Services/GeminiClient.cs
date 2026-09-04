using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class GeminiClient : IAIClient
{
    private readonly HttpClient _http;
    private readonly GeminiSettings _settings;
    private readonly GeminiLogger _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GeminiClient(HttpClient httpClient, IOptions<GeminiSettings> options, GeminiLogger geminiLogger)
    {
        _http = httpClient;
        _settings = options.Value;
        _logger = geminiLogger;
    }

    public async Task<AIResponse> SendAsync(AIPrompt prompt)
    {
        // Gemini implicit caching: identical systemInstruction content is automatically
        // reused across requests on Gemini 2.0 Flash — no extra API calls needed.
        // Explicit caching via POST /cachedContents is available but requires
        // a separate pre-registration step; implicit caching covers chat use cases.
        var body = new GeminiRequest
        {
            SystemInstruction = new GeminiSystemInstruction
            {
                Parts = [new GeminiPart { Text = prompt.SystemMessage }]
            },
            Contents =
            [
                new GeminiContent
                {
                    Role = "user",
                    Parts = [new GeminiPart { Text = prompt.UserMessage }]
                }
            ],
            GenerationConfig = new GeminiGenerationConfig
            {
                Temperature = _settings.Temperature,
                MaxOutputTokens = _settings.MaxTokens,
                ResponseMimeType = prompt.RequireJsonResponse ? "application/json" : null
            }
        };

        var delays = new[] { 15, 30, 60 };

        for (int attempt = 0; ; attempt++)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = JsonContent.Create(body);

            if (attempt == 0)
                await _logger.LogRequestAsync(prompt.SystemMessage, prompt.UserMessage);

            var response = await _http.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < delays.Length)
            {
                await Task.Delay(TimeSpan.FromSeconds(delays[attempt]));
                continue;
            }

            response.EnsureSuccessStatusCode();

            var rawJson = await response.Content.ReadAsStringAsync();
            await _logger.LogResponseAsync(rawJson);

            var result = JsonSerializer.Deserialize<GeminiResponse>(rawJson, _jsonOptions)
                ?? throw new InvalidOperationException("Empty response from Gemini");

            return new AIResponse
            {
                Content    = result.Candidates[0].Content.Parts[0].Text,
                TokensUsed = result.UsageMetadata?.TotalTokenCount ?? 0
            };
        }
    }
}
