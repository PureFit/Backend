using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class ExerciseTranslationService : IExerciseTranslationService
{
    private readonly AppDbContext _db;
    private readonly IAIClient _ai;
    private readonly ILogger<ExerciseTranslationService> _logger;

    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ExerciseTranslationService(AppDbContext db, IAIClient ai, ILogger<ExerciseTranslationService> logger)
    {
        _db = db;
        _ai = ai;
        _logger = logger;
    }

    public async Task PopulateNameRuAsync(CancellationToken ct = default)
    {
        var exercises = await _db.Exercises
            .Where(e => e.NameRu == null)
            .Select(e => new { e.Id, e.Name })
            .OrderBy(e => e.Name)
            .ToListAsync(ct);

        _logger.LogInformation("ExerciseTranslation: {Count} exercises to translate", exercises.Count);

        const int batchSize = 50;

        for (int i = 0; i < exercises.Count; i += batchSize)
        {
            if (ct.IsCancellationRequested) break;

            var batch = exercises.Skip(i).Take(batchSize).ToList();
            var names = batch.Select(e => e.Name).ToList();

            _logger.LogInformation("ExerciseTranslation: batch {Batch}/{Total}", i / batchSize + 1, (exercises.Count - 1) / batchSize + 1);

            var namesJson = JsonSerializer.Serialize(names);
            var prompt = new AIPrompt
            {
                SystemMessage = "You are a fitness terminology translator. Translate English exercise names to Russian using standard fitness terminology. Return ONLY valid JSON object mapping each English name to Russian translation. No markdown, no explanation.",
                UserMessage = $"Translate these exercise names to Russian. Return JSON like {{\"Barbell Squat\": \"Приседания со штангой\"}}.\n\nNames:\n{namesJson}",
                RequireJsonResponse = true
            };

            try
            {
                var response = await _ai.SendAsync(prompt);
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content, _json);

                if (translations == null)
                {
                    _logger.LogWarning("ExerciseTranslation: null response for batch {Batch}", i / batchSize + 1);
                    continue;
                }

                foreach (var ex in batch)
                {
                    if (translations.TryGetValue(ex.Name, out var nameRu) && !string.IsNullOrWhiteSpace(nameRu))
                    {
                        await _db.Exercises
                            .Where(e => e.Id == ex.Id)
                            .ExecuteUpdateAsync(s => s.SetProperty(e => e.NameRu, nameRu), ct);
                    }
                }

                _logger.LogInformation("ExerciseTranslation: batch {Batch} saved ({Count} translations)", i / batchSize + 1, translations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExerciseTranslation: batch {Batch} failed", i / batchSize + 1);
            }

            if (i + batchSize < exercises.Count)
                await Task.Delay(5000, ct);
        }

        _logger.LogInformation("ExerciseTranslation: done");
    }
}
