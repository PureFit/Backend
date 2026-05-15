using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using Backend.Application.Helpers;
using Backend.Application.Repositories;
using Backend.Application.Services;
namespace Backend.Infrastructure.Services;

public class PromptBuilder : IPromptBuilder
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ICacheService _cacheService;

    public PromptBuilder(ICacheService cacheService, IExerciseRepository exerciseRepository)
    {
        _cacheService = cacheService;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<AIPrompt> BuildAsync(GeneratePlanRequest request)
    {
        var exercisesBrief = await _cacheService.GetAsync<List<ExerciseBrief>>(CacheKeys.ExercisesBrief);
        if (exercisesBrief == null)
        {
            exercisesBrief = await _exerciseRepository.GetExercisesBriefAsync();
            await _cacheService.SetAsync<List<ExerciseBrief>>(CacheKeys.ExercisesBrief, exercisesBrief);
        }

        var filtered = FilterExercisesByEquipment(exercisesBrief, request.AvailableEquipment);

        var prompt = PromptHelper
            .CreateBase()
            .AddUserRequest(request)
            .AddExercises(filtered);

        return prompt;
    }

    // Упражнения без оборудования (bodyweight) всегда включаются.
    // Если список оборудования пустой — возвращаем только bodyweight.
    private static List<ExerciseBrief> FilterExercisesByEquipment(
        List<ExerciseBrief> all,
        List<string> availableEquipment)
    {
        return all
            .Where(e => e.Equipment.Count == 0
                     || e.Equipment.Any(eq => availableEquipment
                         .Any(a => a.Equals(eq, StringComparison.OrdinalIgnoreCase))))
            .ToList();
    }
}
