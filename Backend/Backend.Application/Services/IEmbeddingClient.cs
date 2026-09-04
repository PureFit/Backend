namespace Backend.Application.Services;

public interface IEmbeddingClient
{
    Task<float[]> EmbedAsync(string text);
}
