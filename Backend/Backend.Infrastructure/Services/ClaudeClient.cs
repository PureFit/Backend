using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class ClaudeClient : IAIClient
{
    private readonly HttpClient _http;
    private readonly ClaudeSettings _settings;
    private readonly ClaudeLogger _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ClaudeClient(HttpClient httpClient, IOptions<ClaudeSettings> options, ClaudeLogger claudeLogger)
    {
        _http = httpClient;
        _settings = options.Value;
        _logger = claudeLogger;
    }

    public async Task<AIResponse> SendAsync(AIPrompt prompt)
    {
        var body = new ClaudeRequest
        {
            Model = _settings.Model,
            MaxTokens = _settings.MaxTokens,
            Temperature = _settings.Temperature,
            // cache_control tells Claude to cache this system prompt server-side (~5 min TTL).
            // Subsequent requests with identical content cost 10% of normal input token price.
            System =
            [
                new ClaudeSystemBlock
                {
                    Text = prompt.SystemMessage,
                    CacheControl = new ClaudeCacheControl()
                }
            ],
            Messages =
            [
                new() { Role = "user", Content = prompt.UserMessage }
            ]
        };

        var delays = new[] { 15, 30, 60 };

        for (int attempt = 0; ; attempt++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            request.Headers.Add("x-api-key", _settings.ApiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
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

            var result = JsonSerializer.Deserialize<ClaudeResponse>(rawJson, _jsonOptions)
                ?? throw new InvalidOperationException("Empty response from Claude");

            return new AIResponse
            {
                Content    = result.Content.First(b => b.Type == "text").Text,
                TokensUsed = (result.Usage?.InputTokens ?? 0) + (result.Usage?.OutputTokens ?? 0)
            };
        }
    }
}
