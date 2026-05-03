using Backend.Application.DTOs.Excercises;

namespace Backend.Application.Repositories;

public interface IExternalExerciseRepository
{
    Task<ExercisePagedResult> GetExercisesAsync(ExerciseFilter filter);
    Task<ExerciseDetailsDto?> GetExerciseDetailsAsync(string externalId);
    Task<List<string>> GetMusclesAsync();
    Task<List<BodyPartItemDto>> GetBodyPartsAsync();
    Task<List<EquipmentItemDto>> GetEquipmentsAsync();
    Task<List<string>> GetExerciseTypesAsync();
}
