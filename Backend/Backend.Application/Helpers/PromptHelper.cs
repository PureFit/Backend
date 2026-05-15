using Backend.Application.DTOs.Plan;
using System.Text;

namespace Backend.Application.Helpers;

public static class PromptHelper
{
    public static AIPrompt CreateBase() =>
        new AIPrompt
        {
            SystemMessage = PromptConstants.BaseGenerateHeader(),
            UserMessage = string.Empty
        };

    public static AIPrompt AddExercises(this AIPrompt prompt, List<ExerciseBrief> exercises)
    {
        var sb = new StringBuilder(prompt.SystemMessage);
        sb.AppendLine();
        sb.AppendLine("=== EXERCISE CATALOG ===");
        sb.AppendLine("Format: id|name|bodyParts|muscles|equipment|measure");
        sb.AppendLine();

        foreach (var ex in exercises)
            sb.AppendLine(ex.ToCompactString());

        prompt.SystemMessage = sb.ToString();
        return prompt;
    }

    public static AIPrompt AddUserRequest(this AIPrompt prompt, GeneratePlanRequest request)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== USER PROFILE ===");
        if (request.Sex != null)          sb.AppendLine($"Sex: {request.Sex}");
        if (request.AgeYears != null)     sb.AppendLine($"Age: {request.AgeYears} years");
        if (request.WeightKg != null)     sb.AppendLine($"Weight: {request.WeightKg} kg");
        if (request.HeightCm != null)     sb.AppendLine($"Height: {request.HeightCm} cm");
        if (request.FitnessLevel != null) sb.AppendLine($"Fitness level: {request.FitnessLevel}");

        sb.AppendLine();
        sb.AppendLine("=== PLAN PARAMETERS ===");
        sb.AppendLine($"Goal: {request.PlanType} / {request.PlanSubType}");
        sb.AppendLine($"Duration: {request.PlanDurationWeeks} weeks");
        sb.AppendLine($"Sessions per week: {request.SessionsPerWeek}");
        sb.AppendLine($"Session duration: {request.SessionDurationMinutes} min");

        if (request.AvailableEquipment.Count > 0)
            sb.AppendLine($"Equipment: {string.Join(", ", request.AvailableEquipment)}");
        else
            sb.AppendLine("Equipment: bodyweight only");

        if (!string.IsNullOrWhiteSpace(request.GoalMetadata))
        {
            sb.AppendLine();
            sb.AppendLine("=== GOAL DETAILS ===");
            sb.AppendLine(request.GoalMetadata);
        }

        if (!string.IsNullOrWhiteSpace(request.FreeTextWish))
        {
            sb.AppendLine();
            sb.AppendLine("=== ADDITIONAL WISHES ===");
            sb.AppendLine(request.FreeTextWish);
        }

        prompt.UserMessage = sb.ToString();
        return prompt;
    }
}
