using Backend.Application.Services;
using System.Text.Json;

namespace Backend.Infrastructure.Services;

public class ChatToolExecutor : IChatToolExecutor
{
    private readonly IProfileService _profileService;
    private readonly IPlanService _planService;

    public ChatToolExecutor(IProfileService profileService, IPlanService planService)
    {
        _profileService = profileService;
        _planService = planService;
    }

    public Task<string> ExecuteAsync(string toolName, string argumentsJson, Guid userId) =>
        toolName switch
        {
            "get_user_profile" => GetUserProfileAsync(userId),
            "get_active_plan" => GetActivePlanAsync(userId),
            _ => Task.FromResult("{\"error\":\"unknown tool\"}")
        };

    private async Task<string> GetUserProfileAsync(Guid userId)
    {
        var result = await _profileService.GetProfileAsync(userId);
        if (!result.Success || result.Data is null)
            return "{\"error\":\"profile not found\"}";

        var p = result.Data;
        var age = CalculateAge(p.DateOfBirth);

        return JsonSerializer.Serialize(new
        {
            sex = p.Sex,
            age_years = age,
            weight_kg = p.WeightKg,
            height_cm = p.HeightCm,
            fitness_level = p.FitnessLevel
        });
    }

    private async Task<string> GetActivePlanAsync(Guid userId)
    {
        var result = await _planService.GetPlanAsync(userId);
        if (!result.Success || result.Data is null)
            return "{\"no_active_plan\":true}";

        var plan = result.Data;
        var currentWeek = plan.Weeks.FirstOrDefault(w => w.WeekStatus == "InProgress")
                       ?? plan.Weeks.FirstOrDefault();

        return JsonSerializer.Serialize(new
        {
            plan_type = plan.PlanType,
            plan_sub_type = plan.PlanSubType,
            weeks_duration = plan.WeeksDuration,
            current_week = currentWeek?.WeekNumber ?? 1,
            upcoming_trainings = currentWeek?.PlanTrainings.Select(t => new
            {
                planned_date = t.StartPlannedDate,
                training_name = t.TrainingSet?.Name,
                is_completed = t.IsCompleted
            })
        });
    }

    private static int CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;
        return age;
    }
}
