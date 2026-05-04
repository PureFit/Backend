using Backend.Application.Common;
using Backend.Application.DTOs.Excercises;
using Backend.Application.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Application.Services.impl;

public class ExerciseService : IExerciseService
{
    private readonly IExternalExerciseRepository _repository;
    private readonly ILogger<ExerciseService> _logger;
    private readonly ICacheService _cacheService;
    private readonly RedisConfig _redis;

    public ExerciseService(
        IExternalExerciseRepository repository,
        ILogger<ExerciseService> logger,
        ICacheService cacheService,
        IOptions<RedisConfig> redisOptions)
    {
        _repository = repository;
        _logger = logger;
        _cacheService = cacheService;
        _redis = redisOptions.Value;
    }

    public async Task<BaseResponse<ExercisePagedResult>> GetExercisesAsync(ExerciseFilter filter)
    {
        try
        {
            var cacheKey = CacheKeys.Exercises(filter.Keywords, filter.BodyPart, filter.Muscle,
                                               filter.Equipment, filter.Type, filter.Cursor, filter.Limit);

            var cached = await _cacheService.GetAsync<ExercisePagedResult>(cacheKey);
            if (cached != null)
                return BaseResponse<ExercisePagedResult>.Ok(cached);

            var result = await _repository.GetExercisesAsync(filter);
            if (result.Items.Count > 0)
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(_redis.ExerciseListTtlHours));

            return BaseResponse<ExercisePagedResult>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercises");
            return BaseResponse<ExercisePagedResult>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<ExerciseDetailsDto>> GetExerciseByIdAsync(string id)
    {
        try
        {
            var cacheKey = CacheKeys.ExerciseDetail(id);

            var cached = await _cacheService.GetAsync<ExerciseDetailsDto>(cacheKey);
            if (cached != null)
                return BaseResponse<ExerciseDetailsDto>.Ok(cached);

            var result = await _repository.GetExerciseDetailsAsync(id);
            if (result == null)
                return BaseResponse<ExerciseDetailsDto>.Fail("Exercise not found");

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(_redis.ExerciseDetailTtlHours));
            return BaseResponse<ExerciseDetailsDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercise {Id}", id);
            return BaseResponse<ExerciseDetailsDto>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<string>>> GetMusclesAsync()
    {
        try
        {
            var cached = await _cacheService.GetAsync<List<string>>(CacheKeys.Muscles);
            if (cached != null)
                return BaseResponse<List<string>>.Ok(cached);

            var result = await _repository.GetMusclesAsync();
            if (result.Count > 0)
                await _cacheService.SetAsync(CacheKeys.Muscles, result, TimeSpan.FromHours(_redis.StaticDataTtlHours));

            return BaseResponse<List<string>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get muscles");
            return BaseResponse<List<string>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<BodyPartItemDto>>> GetBodyPartsAsync()
    {
        try
        {
            var cached = await _cacheService.GetAsync<List<BodyPartItemDto>>(CacheKeys.BodyParts);
            if (cached != null)
                return BaseResponse<List<BodyPartItemDto>>.Ok(cached);

            var result = await _repository.GetBodyPartsAsync();
            if (result.Count > 0)
                await _cacheService.SetAsync(CacheKeys.BodyParts, result, TimeSpan.FromHours(_redis.StaticDataTtlHours));

            return BaseResponse<List<BodyPartItemDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get body parts");
            return BaseResponse<List<BodyPartItemDto>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<EquipmentItemDto>>> GetEquipmentsAsync()
    {
        try
        {
            var cached = await _cacheService.GetAsync<List<EquipmentItemDto>>(CacheKeys.Equipments);
            if (cached != null)
                return BaseResponse<List<EquipmentItemDto>>.Ok(cached);

            var result = await _repository.GetEquipmentsAsync();
            if (result.Count > 0)
                await _cacheService.SetAsync(CacheKeys.Equipments, result, TimeSpan.FromHours(_redis.StaticDataTtlHours));

            return BaseResponse<List<EquipmentItemDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipments");
            return BaseResponse<List<EquipmentItemDto>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<string>>> GetExerciseTypesAsync()
    {
        try
        {
            var cached = await _cacheService.GetAsync<List<string>>(CacheKeys.ExerciseTypes);
            if (cached != null)
                return BaseResponse<List<string>>.Ok(cached);

            var result = await _repository.GetExerciseTypesAsync();
            if (result.Count > 0)
                await _cacheService.SetAsync(CacheKeys.ExerciseTypes, result, TimeSpan.FromHours(_redis.StaticDataTtlHours));

            return BaseResponse<List<string>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercise types");
            return BaseResponse<List<string>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }
}
