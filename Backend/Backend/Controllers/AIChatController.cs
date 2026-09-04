using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/ai/chat")]
public class AIChatController : BaseController
{
    private readonly IAIService _aiService;
    private readonly IChatPromptBuilder _promptBuilder;
    private readonly IChatContextCache _contextCache;
    private readonly IChatHistoryCache _historyCache;
    private readonly IProfileService _profileService;
    private readonly IPlanService _planService;

    public AIChatController(
        IAIService aiService,
        IChatPromptBuilder promptBuilder,
        IChatContextCache contextCache,
        IChatHistoryCache historyCache,
        IProfileService profileService,
        IPlanService planService)
    {
        _aiService = aiService;
        _promptBuilder = promptBuilder;
        _contextCache = contextCache;
        _historyCache = historyCache;
        _profileService = profileService;
        _planService = planService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] AIChatRequest request)
    {
        var userId = GetUserIdFromClaims();

        // Load context (profile + plan) — from Redis or build fresh
        var context = await _contextCache.GetAsync(userId);
        if (context == null)
        {
            context = await BuildContextAsync(userId);
            await _contextCache.SetAsync(userId, context);
        }

        // Load conversation history from Redis
        var history = await _historyCache.GetAsync(userId);

        // Build prompt
        var prompt = _promptBuilder.Build(request, context, history);

        // Call AI
        var reply = await _aiService.ChatAsync(prompt);

        // Persist new messages to history
        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "user",      Content = request.Message });
        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "assistant", Content = reply });

        return Ok(BaseResponse<AIChatResponse>.Ok(new AIChatResponse { Message = reply }));
    }

    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory()
    {
        var userId = GetUserIdFromClaims();
        await _historyCache.ClearAsync(userId);
        return Ok(BaseResponse<bool>.Ok(true));
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<UserChatContext> BuildContextAsync(Guid userId)
    {
        var context = new UserChatContext();

        var profileResult = await _profileService.GetProfileAsync(userId);
        if (profileResult.Success && profileResult.Data is { } profile)
        {
            context.Sex          = profile.Sex;
            context.WeightKg     = profile.WeightKg;
            context.HeightCm     = profile.HeightCm;
            context.FitnessLevel = profile.FitnessLevel;
            context.AgeYears     = CalculateAge(profile.DateOfBirth);
        }

        var planResult = await _planService.GetPlanAsync(userId);
        if (planResult.Success && planResult.Data is { } plan)
        {
            var currentWeek = plan.Weeks.FirstOrDefault(w => w.WeekStatus == "InProgress")
                           ?? plan.Weeks.FirstOrDefault();

            context.ActivePlan = new ActivePlanSummary
            {
                PlanType      = plan.PlanType,
                PlanSubType   = plan.PlanSubType,
                WeeksDuration = plan.WeeksDuration,
                CurrentWeek   = currentWeek?.WeekNumber ?? 1,
                UpcomingTrainings = currentWeek?.PlanTrainings
                    .Select(t => new UpcomingTrainingSummary
                    {
                        PlannedDate     = t.StartPlannedDate,
                        TrainingSetName = t.TrainingSet?.Name,
                        IsCompleted     = t.IsCompleted
                    })
                    .ToList() ?? []
            };
        }

        return context;
    }

    private static int? CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;
        return age;
    }
}
