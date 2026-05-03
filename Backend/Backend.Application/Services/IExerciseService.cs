using Backend.Application.Common;
using Backend.Application.DTOs.Excercises;

namespace Backend.Application.Services;

public interface IExerciseService
{
    Task<BaseResponse<ExercisePagedResult>> GetExercisesAsync(ExerciseFilter filter);
    Task<BaseResponse<ExerciseDetailsDto>> GetExerciseByIdAsync(string id);
    Task<BaseResponse<List<string>>> GetMusclesAsync();
    Task<BaseResponse<List<BodyPartItemDto>>> GetBodyPartsAsync();
    Task<BaseResponse<List<EquipmentItemDto>>> GetEquipmentsAsync();
    Task<BaseResponse<List<string>>> GetExerciseTypesAsync();
}
