namespace Backend.Application.Services;

public interface IExerciseEmbeddingService
{
    /// <summary>
    /// Генерирует и сохраняет эмбеддинги для всех упражнений в БД.
    /// Вызывается один раз вручную через admin-endpoint.
    /// </summary>
    Task PopulateAllAsync(CancellationToken ct = default);
}
