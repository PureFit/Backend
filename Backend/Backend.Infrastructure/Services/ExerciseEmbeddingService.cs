using Backend.Application.DTOs.Plan;
using Backend.Application.Repositories;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class ExerciseEmbeddingService : IExerciseEmbeddingService
{
    private readonly IExerciseRepository _repository;
    private readonly IEmbeddingClient _embeddingClient;

    public ExerciseEmbeddingService(IExerciseRepository repository, IEmbeddingClient embeddingClient)
    {
        _repository = repository;
        _embeddingClient = embeddingClient;
    }

    public async Task PopulateAllAsync(CancellationToken ct = default)
    {
        var exercises = await _repository.GetExercisesForEmbeddingAsync();

        // Gemini embedding API: free tier ~1500 req/min — батчи по 20 с паузой 300ms
        const int batchSize = 20;
        for (int i = 0; i < exercises.Count; i += batchSize)
        {
            if (ct.IsCancellationRequested) break;

            var batch = exercises.Skip(i).Take(batchSize).ToList();

            foreach (var ex in batch)
            {
                var text = BuildEmbedText(ex);
                var embedding = await _embeddingClient.EmbedAsync(text);
                await _repository.UpsertEmbeddingAsync(ex.Id, embedding);
            }

            if (i + batchSize < exercises.Count)
                await Task.Delay(300, ct);
        }
    }

    private static string BuildEmbedText(ExerciseEmbedData ex)
    {
        var parts = new List<string> { ex.Name, ex.Category };

        if (ex.Muscles.Count > 0)
            parts.Add($"muscles: {string.Join(", ", ex.Muscles)}");
        if (ex.BodyParts.Count > 0)
            parts.Add($"body parts: {string.Join(", ", ex.BodyParts)}");
        if (ex.Equipment.Count > 0)
            parts.Add($"equipment: {string.Join(", ", ex.Equipment)}");
        if (ex.Keywords.Count > 0)
            parts.Add(string.Join(", ", ex.Keywords));

        return string.Join(" ", parts);
    }
}
