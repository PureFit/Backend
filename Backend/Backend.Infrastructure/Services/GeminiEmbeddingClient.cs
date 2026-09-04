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

    private const string Model = "text-embedding-004";

    public GeminiEmbeddingClient(HttpClient http, IOptions<GeminiSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:embedContent?key={_settings.ApiKey}";
        var body = new { model = $"models/{Model}", content = new { parts = new[] { new { text } } } };

        var response = await _http.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GeminiEmbedResponse>();
        return result!.Embedding.Values;
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
