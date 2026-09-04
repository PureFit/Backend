using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;

namespace Backend.Infrastructure.Services;

public class AIChatService : IAIChatService
{
    private readonly IAIService _aiService;
    private readonly IChatPromptBuilder _promptBuilder;
    private readonly IChatHistoryCache _historyCache;
    private readonly IUsageLimiterService _usageLimiter;
    private readonly IProfileService _profileService;
    private readonly IPlanService _planService;

    public AIChatService(
        IAIService aiService,
        IChatPromptBuilder promptBuilder,
        IChatHistoryCache historyCache,
        IUsageLimiterService usageLimiter,
        IProfileService profileService,
        IPlanService planService)
    {
        _aiService      = aiService;
        _promptBuilder  = promptBuilder;
        _historyCache   = historyCache;
        _usageLimiter   = usageLimiter;
        _profileService = profileService;
        _planService    = planService;
    }

    public async Task<BaseResponse<AIChatResponse>> ChatAsync(Guid userId, AIChatRequest request)
    {
        var denied = await _usageLimiter.CheckAsync(userId);
        if (denied.HasValue)
            return BaseResponse<AIChatResponse>.Fail(denied.Value);

        var context = await BuildContextAsync(userId);
        var history  = await _historyCache.GetAsync(userId);
        var prompt   = _promptBuilder.Build(request, context, history);
        var response = await _aiService.ChatAsync(prompt);

        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "user",      Content = request.Message });
        await _historyCache.AppendAsync(userId, new ChatMessageDto { Role = "assistant", Content = response.Content });
        await _usageLimiter.RecordAsync(userId, response.TokensUsed);

        return BaseResponse<AIChatResponse>.Ok(new AIChatResponse { Message = response.Content });
    }

    public async Task ClearHistoryAsync(Guid userId)
    {
        await _historyCache.ClearAsync(userId);
    }

    public Task<UserUsageDto> GetUsageAsync(Guid userId) =>
        _usageLimiter.GetUsageAsync(userId);

    // ── private ──────────────────────────────────────────────────────────────

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
