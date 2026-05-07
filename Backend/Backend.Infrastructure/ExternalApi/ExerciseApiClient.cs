using System.Net.Http.Json;
using System.Text;
using Backend.Application.DTOs.Excercises;
using Backend.Application.Repositories;
using Backend.Infrastructure.ExternalApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.ExternalApi;

public class ExerciseApiClient : IExternalExerciseRepository
{
    private readonly HttpClient _httpClient;
    private readonly ExerciseApiSettings _settings;
    private readonly ILogger<ExerciseApiClient> _logger;

    public ExerciseApiClient(
        HttpClient httpClient,
        IOptions<ExerciseApiSettings> settings,
        ILogger<ExerciseApiClient> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        httpClient.BaseAddress = new Uri(_settings.BaseUrl.TrimEnd('/') + "/");
        httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", _settings.ApiKey);
        httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", _settings.ApiHost);
        _httpClient = httpClient;
    }

    public async Task<ExercisePagedResult> GetExercisesAsync(ExerciseFilter filter)
    {
        var query = new StringBuilder("exercises?");
        if (!string.IsNullOrEmpty(filter.Keywords))  query.Append($"keywords={Uri.EscapeDataString(filter.Keywords)}&");
        if (!string.IsNullOrEmpty(filter.BodyPart))  query.Append($"bodyParts={Uri.EscapeDataString(filter.BodyPart)}&");
        if (!string.IsNullOrEmpty(filter.Muscle))    query.Append($"targetMuscles={Uri.EscapeDataString(filter.Muscle)}&");
        if (!string.IsNullOrEmpty(filter.Equipment)) query.Append($"equipments={Uri.EscapeDataString(filter.Equipment)}&");
        if (!string.IsNullOrEmpty(filter.Type))      query.Append($"exerciseType={Uri.EscapeDataString(filter.Type)}&");
        if (!string.IsNullOrEmpty(filter.Cursor))    query.Append($"after={Uri.EscapeDataString(filter.Cursor)}&");
        query.Append($"limit={filter.Limit}");

        var response = await _httpClient.GetFromJsonAsync<ExerciseApiResponse>(query.ToString());

        if (response == null || !response.Success)
            return new ExercisePagedResult();

        _logger.LogInformation(
            "ExerciseApi meta: total={Total}, hasNextPage={HasNextPage}, nextCursor={NextCursor}",
            response.Meta.Total, response.Meta.HasNextPage, response.Meta.NextCursor ?? "NULL");

        return new ExercisePagedResult
        {
            Items = response.Data.Select(MapToDto).ToList(),
            HasNextPage = response.Meta.HasNextPage,
            NextCursor = response.Meta.NextCursor,
            Total = response.Meta.Total
        };
    }

    public async Task<ExerciseDetailsDto?> GetExerciseDetailsAsync(string externalId)
    {
        var response = await _httpClient.GetFromJsonAsync<DetailedExerciseResponse>($"exercises/{externalId}");
        if (response == null || !response.Success) return null;
        _logger.LogInformation("Exercise detail: id={Id}, videoUrl={VideoUrl}, imageUrl={ImageUrl}",
            response.Data.ExerciseId, response.Data.VideoUrl ?? "NULL", response.Data.ImageUrl);
        return MapToDetailsDto(response.Data);
    }

    public async Task<List<string>> GetMusclesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<MuscleListResponse>("muscles");
        return response?.Data.Select(m => m.Name).ToList() ?? new();
    }

    public async Task<List<BodyPartItemDto>> GetBodyPartsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<BodyPartListResponse>("body-parts");
        return response?.Data.Select(b => new BodyPartItemDto
        {
            Name = b.Name,
            ImageUrl = b.ImageUrl
        }).ToList() ?? new();
    }

    public async Task<List<EquipmentItemDto>> GetEquipmentsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<EquipmentListResponse>("equipments");
        return response?.Data.Select(e => new EquipmentItemDto
        {
            Name = e.Name,
            ImageUrl = e.ImageUrl
        }).ToList() ?? new();
    }

    public async Task<List<string>> GetExerciseTypesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ExerciseTypeListResponse>("exercise-types");
        return response?.Data.Select(t => t.Name).ToList() ?? new();
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static ExerciseDto MapToDto(ExternalExerciseDto src) => new()
    {
        Id = src.ExerciseId,
        Name = src.Name,
        ImageUrl = src.ImageUrl,
        BodyParts = src.BodyParts,
        TargetMuscles = src.TargetMuscles,
        Equipments = src.Equipments,
        ExerciseType = src.ExerciseType
    };

    private static ExerciseDetailsDto MapToDetailsDto(DetailedExerciseDto src) => new()
    {
        Id = src.ExerciseId,
        Name = src.Name,
        ImageUrl = src.ImageUrl,
        ImageUrl480p = src.ImageUrls?.P480,
        ImageUrl720p = src.ImageUrls?.P720,
        VideoUrl = src.VideoUrl,
        Overview = src.Overview,
        BodyParts = src.BodyParts,
        TargetMuscles = src.TargetMuscles,
        SecondaryMuscles = src.SecondaryMuscles,
        Equipments = src.Equipments,
        ExerciseType = src.ExerciseType,
        Instructions = src.Instructions,
        ExerciseTips = src.ExerciseTips,
        Variations = src.Variations,
        Keywords = src.Keywords,
        RelatedExerciseIds = src.RelatedExerciseIds
    };
}
