using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class MetadataTranslationService : IMetadataTranslationService
{
    private readonly AppDbContext _db;
    private readonly GroqClient _ai;
    private readonly ILogger<MetadataTranslationService> _logger;

    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public MetadataTranslationService(AppDbContext db, GroqClient ai, ILogger<MetadataTranslationService> logger)
    {
        _db = db;
        _ai = ai;
        _logger = logger;
    }

    public async Task PopulateAsync(CancellationToken ct = default)
    {
        await TranslateBodyPartsAsync(ct);
        await TranslateMusclesAsync(ct);
        await TranslateEquipmentAsync(ct);
        _logger.LogInformation("MetadataTranslation: all done");
    }

    private async Task TranslateBodyPartsAsync(CancellationToken ct)
    {
        var items = await _db.BodyParts
            .Where(x => x.NameRu == null)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(ct);

        _logger.LogInformation("MetadataTranslation: {Count} body parts to translate", items.Count);
        if (items.Count == 0) return;

        var translations = await TranslateBatchAsync(items.Select(x => x.Name).ToList(), ct);
        if (translations == null) return;

        foreach (var item in items)
        {
            if (translations.TryGetValue(item.Name, out var nameRu) && !string.IsNullOrWhiteSpace(nameRu))
                await _db.BodyParts.Where(x => x.Id == item.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.NameRu, nameRu), ct);
        }

        _logger.LogInformation("MetadataTranslation: body parts saved");
    }

    private async Task TranslateMusclesAsync(CancellationToken ct)
    {
        var items = await _db.Muscles
            .Where(x => x.NameRu == null)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(ct);

        _logger.LogInformation("MetadataTranslation: {Count} muscles to translate", items.Count);
        if (items.Count == 0) return;

        var translations = await TranslateBatchAsync(items.Select(x => x.Name).ToList(), ct);
        if (translations == null) return;

        foreach (var item in items)
        {
            if (translations.TryGetValue(item.Name, out var nameRu) && !string.IsNullOrWhiteSpace(nameRu))
                await _db.Muscles.Where(x => x.Id == item.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.NameRu, nameRu), ct);
        }

        _logger.LogInformation("MetadataTranslation: muscles saved");
    }

    private async Task TranslateEquipmentAsync(CancellationToken ct)
    {
        var items = await _db.Equipments
            .Where(x => x.NameRu == null)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(ct);

        _logger.LogInformation("MetadataTranslation: {Count} equipment to translate", items.Count);
        if (items.Count == 0) return;

        var translations = await TranslateBatchAsync(items.Select(x => x.Name).ToList(), ct);
        if (translations == null) return;

        foreach (var item in items)
        {
            if (translations.TryGetValue(item.Name, out var nameRu) && !string.IsNullOrWhiteSpace(nameRu))
                await _db.Equipments.Where(x => x.Id == item.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.NameRu, nameRu), ct);
        }

        _logger.LogInformation("MetadataTranslation: equipment saved");
    }

    private async Task<Dictionary<string, string>?> TranslateBatchAsync(List<string> names, CancellationToken ct)
    {
        var namesJson = JsonSerializer.Serialize(names);
        var prompt = new AIPrompt
        {
            SystemMessage = "You are a fitness terminology translator. Translate English fitness terms to Russian using standard sports terminology. Return ONLY valid JSON object mapping each English name to Russian translation. No markdown, no explanation.",
            UserMessage = $"Translate these fitness terms to Russian. Return JSON like {{\"Chest\": \"Грудь\"}}.\n\nTerms:\n{namesJson}",
            RequireJsonResponse = false
        };

        try
        {
            var response = await _ai.SendAsync(prompt);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content, _json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MetadataTranslation: batch failed");
            return null;
        }
    }
}
