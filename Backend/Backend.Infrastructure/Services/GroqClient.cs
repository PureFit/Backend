using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
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

    public async Task<AIResponse> SendAsync(AIPrompt prompt)
    {
        var body = new GroqRequest
        {
            Model       = _settings.Model,
            Temperature = _settings.Temperature,
            MaxTokens   = _settings.MaxTokens,
            Messages    = BuildMessages(prompt),
            ResponseFormat = prompt.RequireJsonResponse ? new GroqResponseFormat() : null,
            Tools = prompt.Tools?.Select(t => new GroqTool
            {
                Function = new GroqFunction
                {
                    Name        = t.Name,
                    Description = t.Description,
                    Parameters  = t.Parameters
                }
            }).ToList()
        };

        var delays = new[] { 15, 30, 60 };

        for (int attempt = 0; ; attempt++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
            request.Content = JsonContent.Create(body);

            if (attempt == 0)
                await _logger.LogRequestAsync(prompt.SystemMessage, prompt.Messages != null
                    ? $"[structured messages: {prompt.Messages.Count}]"
                    : prompt.UserMessage ?? "");

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

            var choice = result.Choices[0];

            if (choice.FinishReason == "tool_calls" && choice.Message.ToolCalls?.Count > 0)
            {
                return new AIResponse
                {
                    TokensUsed = result.Usage?.TotalTokens ?? 0,
                    ToolCalls  = choice.Message.ToolCalls.Select(tc => new ToolCallDto
                    {
                        Id           = tc.Id,
                        Name         = tc.Function.Name,
                        ArgumentsJson = tc.Function.Arguments
                    }).ToList()
                };
            }

            return new AIResponse
            {
                Content    = choice.Message.Content,
                TokensUsed = result.Usage?.TotalTokens ?? 0
            };
        }
    }

    private static List<GroqMessage> BuildMessages(AIPrompt prompt)
    {
        var messages = new List<GroqMessage>
        {
            new() { Role = "system", Content = prompt.SystemMessage }
        };

        if (prompt.Messages != null)
        {
            foreach (var msg in prompt.Messages)
            {
                messages.Add(new GroqMessage
                {
                    Role      = msg.Role,
                    Content   = msg.Content,
                    ToolCalls = msg.ToolCalls?.Select(tc => new GroqToolCall
                    {
                        Id       = tc.Id,
                        Function = new GroqToolCallFunction
                        {
                            Name      = tc.Name,
                            Arguments = tc.ArgumentsJson
                        }
                    }).ToList(),
                    ToolCallId = msg.ToolCallId
                });
            }
        }
        else if (prompt.UserMessage != null)
        {
            messages.Add(new() { Role = "user", Content = prompt.UserMessage });
        }

        return messages;
    }
}
