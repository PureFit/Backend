using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using System.Text;

namespace Backend.Infrastructure.Services;

public class ChatPromptBuilder : IChatPromptBuilder
{
    public AIPrompt Build(AIChatRequest request, UserChatContext context, List<ChatMessageDto> history)
    {
        return new AIPrompt
        {
            SystemMessage = BuildSystemMessage(context),
            UserMessage = BuildUserMessage(request, history)
        };
    }

    private static string BuildSystemMessage(UserChatContext ctx)
    {
        var sb = new StringBuilder();

        sb.AppendLine("You are PureFit AI Trainer — a knowledgeable, supportive personal trainer and nutrition coach.");
        sb.AppendLine("Answer questions about workouts, exercise technique, nutrition, recovery, and fitness goals.");
        sb.AppendLine("Keep responses practical, concise, and tailored to the user's profile.");
        sb.AppendLine("If the user has an active training plan, take it into account when giving advice.");
        sb.AppendLine();

        // Profile
        sb.AppendLine("=== USER PROFILE ===");
        if (ctx.Sex != null)          sb.AppendLine($"Sex: {ctx.Sex}");
        if (ctx.AgeYears != null)     sb.AppendLine($"Age: {ctx.AgeYears} years");
        if (ctx.WeightKg != null)     sb.AppendLine($"Weight: {ctx.WeightKg} kg");
        if (ctx.HeightCm != null)     sb.AppendLine($"Height: {ctx.HeightCm} cm");
        if (ctx.FitnessLevel != null) sb.AppendLine($"Fitness level: {ctx.FitnessLevel}");

        // Active plan
        if (ctx.ActivePlan != null)
        {
            var plan = ctx.ActivePlan;
            sb.AppendLine();
            sb.AppendLine("=== ACTIVE TRAINING PLAN ===");
            sb.AppendLine($"Type: {plan.PlanType} / {plan.PlanSubType}");
            sb.AppendLine($"Duration: {plan.WeeksDuration} weeks, currently on week {plan.CurrentWeek}");

            if (plan.UpcomingTrainings.Count > 0)
            {
                sb.AppendLine("Upcoming trainings:");
                foreach (var t in plan.UpcomingTrainings)
                {
                    var status = t.IsCompleted ? "[done]" : "[planned]";
                    var name = t.TrainingSetName ?? "unnamed";
                    sb.AppendLine($"  {status} {t.PlannedDate} — {name}");
                }
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("=== ACTIVE TRAINING PLAN ===");
            sb.AppendLine("No active plan.");
        }

        return sb.ToString();
    }

    private static string BuildUserMessage(AIChatRequest request, List<ChatMessageDto> history)
    {
        if (history.Count == 0)
            return request.Message;

        var sb = new StringBuilder();

        foreach (var msg in history)
        {
            var role = msg.Role.ToLower() == "assistant" ? "Assistant" : "User";
            sb.AppendLine($"{role}: {msg.Content}");
        }

        sb.AppendLine($"User: {request.Message}");

        return sb.ToString();
    }
}
