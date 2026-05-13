using Backend.Application.Common;
using Backend.Application.DTOs.Excercises;

namespace Backend.Application.Services;

public interface IExerciseService
{
    Task<BaseResponse<ExercisePagedResult>> GetExercisesAsync(ExerciseFilter filter);
    Task<BaseResponse<ExerciseDetailsDto>> GetExerciseByIdAsync(Guid id);
    Task<BaseResponse<List<BodyPartItemDto>>> GetBodyPartsAsync();
    Task<BaseResponse<List<EquipmentItemDto>>> GetEquipmentsAsync();
    Task<BaseResponse<List<MuscleDto>>> GetMusclesAsync();
}
