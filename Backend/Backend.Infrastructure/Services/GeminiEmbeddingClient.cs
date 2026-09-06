using Backend.Application.Common;
using Backend.Application.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Backend.Infrastructure.Services;

public class GeminiEmbeddingClient : IEmbeddingClient
{
    private readonly HttpClient _http;
    private readonly GeminiSettings _settings;

    public GeminiEmbeddingClient(HttpClient http, IOptions<GeminiSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<float[]> EmbedAsync(string text, EmbeddingTaskType taskType = EmbeddingTaskType.RetrievalDocument)
    {
        var model = _settings.EmbeddingModel;
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:embedContent?key={_settings.ApiKey}";
        var taskTypeStr = taskType == EmbeddingTaskType.RetrievalQuery ? "RETRIEVAL_QUERY" : "RETRIEVAL_DOCUMENT";
        var body = new { model = $"models/{model}", taskType = taskTypeStr, content = new { parts = new[] { new { text } } } };

        var delays = new[] { 10, 20, 40 }; // секунды

        for (int attempt = 0; ; attempt++)
        {
            var response = await _http.PostAsJsonAsync(url, body);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeminiEmbedResponse>();
                return result!.Embedding.Values;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < delays.Length)
            {
                await Task.Delay(TimeSpan.FromSeconds(delays[attempt]));
                continue;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini embedding failed {(int)response.StatusCode}: {error}");
        }
    }
}

file class GeminiEmbedResponse
{
    [JsonPropertyName("embedding")]
    public GeminiEmbedding Embedding { get; set; } = null!;
}

file class GeminiEmbedding
{
    [JsonPropertyName("values")]
    public float[] Values { get; set; } = [];
}
