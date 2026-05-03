using Backend.Application.Common;
using Backend.Application.DTOs.Excercises;
using Backend.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ExerciseService : IExerciseService
{
    private readonly IExternalExerciseRepository _repository;
    private readonly ILogger<ExerciseService> _logger;

    public ExerciseService(IExternalExerciseRepository repository, ILogger<ExerciseService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BaseResponse<ExercisePagedResult>> GetExercisesAsync(ExerciseFilter filter)
    {
        try
        {
            var result = await _repository.GetExercisesAsync(filter);
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
            var result = await _repository.GetExerciseDetailsAsync(id);
            if (result == null)
                return BaseResponse<ExerciseDetailsDto>.Fail("Exercise not found");
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
            return BaseResponse<List<string>>.Ok(await _repository.GetMusclesAsync());
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
            return BaseResponse<List<BodyPartItemDto>>.Ok(await _repository.GetBodyPartsAsync());
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
            return BaseResponse<List<EquipmentItemDto>>.Ok(await _repository.GetEquipmentsAsync());
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
            return BaseResponse<List<string>>.Ok(await _repository.GetExerciseTypesAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercise types");
            return BaseResponse<List<string>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }
}
