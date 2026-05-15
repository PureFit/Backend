using Backend.Application.DTOs.Excercises;
using Backend.Application.DTOs.Plan;
using Backend.Core.Entities.ExerciseRelated;

namespace Backend.Application.Repositories;

public interface IExerciseRepository
{
    Task<ExercisePagedResult> GetExercisesAsync(ExerciseFilter filter);
    Task<ExerciseDetailsDto?> GetExerciseByIdAsync(Guid id);
    Task<List<BodyPartItemDto>> GetBodyPartsAsync();
    Task<List<EquipmentItemDto>> GetEquipmentsAsync();
    Task<List<MuscleDto>> GetMusclesAsync();
    Task<ExerciseType?> GetExerciseTypeAsync(Guid typeId);
    Task<List<ExerciseBrief>> GetExercisesBriefAsync();
}
