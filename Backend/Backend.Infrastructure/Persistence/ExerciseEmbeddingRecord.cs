using Pgvector;

namespace Backend.Infrastructure.Persistence;

/// <summary>
/// EF-only entity — хранит 768-мерный вектор для каждого упражнения.
/// Не выходит за пределы Infrastructure (Core остаётся чистым).
/// </summary>
public class ExerciseEmbeddingRecord
{
    public Guid ExerciseId { get; set; }
    public Vector Embedding { get; set; } = null!;
}
