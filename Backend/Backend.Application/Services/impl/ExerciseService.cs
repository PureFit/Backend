using Backend.Application.Common;
using Backend.Application.DTOs.Excercises;
using Backend.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly ILogger<ExerciseService> _logger;
    private readonly ICacheService _cacheService;

    public ExerciseService(IExerciseRepository repository, ILogger<ExerciseService> logger, ICacheService cacheService)
    {
        _repository = repository;
        _logger = logger;
        _cacheService = cacheService;

    }

    public async Task<BaseResponse<ExercisePagedResult>> GetExercisesAsync(ExerciseFilter filter)
    {
        try
        {
            var cacheKey = CacheKeys.Exercises(filter);
            var cache = await _cacheService.GetAsync<ExercisePagedResult>(cacheKey);
            if (cache is null)
            {
                var result = await _repository.GetExercisesAsync(filter);
                await _cacheService.SetAsync(cacheKey, result);
                return BaseResponse<ExercisePagedResult>.Ok(result);
            }

            return BaseResponse<ExercisePagedResult>.Ok(cache);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercises");
            return BaseResponse<ExercisePagedResult>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<ExerciseDetailsDto>> GetExerciseByIdAsync(Guid id)
    {
        try
        {
            var cacheKey = CacheKeys.ExerciseDetail(id.ToString());
            var cache = await _cacheService.GetAsync<ExerciseDetailsDto>(cacheKey);

            if (cache is null)
            {
                var result = await _repository.GetExerciseByIdAsync(id);
                if (result is null)
                    return BaseResponse<ExerciseDetailsDto>.Fail(ErrorEnums.NotFound);
                await _cacheService.SetAsync(cacheKey, result);
                return BaseResponse<ExerciseDetailsDto>.Ok(result);
            }

            return BaseResponse<ExerciseDetailsDto>.Ok(cache);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercise {Id}", id);
            return BaseResponse<ExerciseDetailsDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<BodyPartItemDto>>> GetBodyPartsAsync()
    {
        try
        {
            var cache = await _cacheService.GetAsync<List<BodyPartItemDto>>(CacheKeys.BodyParts);
            if (cache is not null) return BaseResponse<List<BodyPartItemDto>>.Ok(cache);

            var result = await _repository.GetBodyPartsAsync();
            await _cacheService.SetAsync(CacheKeys.BodyParts, result, TimeSpan.FromDays(7));
            return BaseResponse<List<BodyPartItemDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get body parts");
            return BaseResponse<List<BodyPartItemDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<EquipmentItemDto>>> GetEquipmentsAsync()
    {
        try
        {
            var cache = await _cacheService.GetAsync<List<EquipmentItemDto>>(CacheKeys.Equipments);
            if (cache is not null) return BaseResponse<List<EquipmentItemDto>>.Ok(cache);

            var result = await _repository.GetEquipmentsAsync();
            await _cacheService.SetAsync(CacheKeys.Equipments, result, TimeSpan.FromDays(7));
            return BaseResponse<List<EquipmentItemDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipments");
            return BaseResponse<List<EquipmentItemDto>>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<List<MuscleDto>>> GetMusclesAsync()
    {
        try
        {
            var cache = await _cacheService.GetAsync<List<MuscleDto>>(CacheKeys.Muscles);
            if (cache is not null) return BaseResponse<List<MuscleDto>>.Ok(cache);

            var result = await _repository.GetMusclesAsync();
            await _cacheService.SetAsync(CacheKeys.Muscles, result, TimeSpan.FromDays(7));
            return BaseResponse<List<MuscleDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get muscles");
            return BaseResponse<List<MuscleDto>>.Fail(ErrorEnums.UnknownError);
        }
    }
}
