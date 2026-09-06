namespace Backend.Application.Services;

public enum EmbeddingTaskType
{
    RetrievalDocument,
    RetrievalQuery
}

public interface IEmbeddingClient
{
    Task<float[]> EmbedAsync(string text, EmbeddingTaskType taskType = EmbeddingTaskType.RetrievalDocument);
}
