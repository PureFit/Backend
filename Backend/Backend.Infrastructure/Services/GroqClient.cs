using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class GroqClient : IAIClient
{
    private readonly HttpClient _http;
    private readonly GroqSettings _settings;
    private readonly GroqLogger _logger;

    public GroqClient(HttpClient httpClient, IOptions<GroqSettings> options, GroqLogger groqLogger)
    {
        _http = httpClient;
        _settings = options.Value;
        _logger = groqLogger;
    }

    public async Task<string> SendAsync(AIPrompt prompt)
    {
        var body = new GroqRequest
        {
            Model = _settings.Model,
            Temperature = _settings.Temperature,
            MaxTokens = _settings.MaxTokens,
            Messages =
            [
                new() { Role = "system", Content = prompt.SystemMessage },
                new() { Role = "user",   Content = prompt.UserMessage   }
            ]
        };

        var delays = new[] { 15, 30, 60 };

        for (int attempt = 0; ; attempt++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
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

            var result = JsonSerializer.Deserialize<GroqResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("Empty response from Groq");

            return result.Choices[0].Message.Content;
        }
    }
}
