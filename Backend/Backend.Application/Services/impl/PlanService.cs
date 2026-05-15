using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class PlanService : IPlanService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IPlanGenerator _planGenerator;
    private readonly IPlanScheduler _planScheduler;
    private readonly ILogger<PlanService> _logger;

    public PlanService(
        IUserInfoRepository userInfoRepository,
        IPlanRepository planRepository,
        IPlanGenerator planGenerator,
        IPlanScheduler planScheduler,
        ILogger<PlanService> logger)
    {
        _userInfoRepository = userInfoRepository;
        _planRepository = planRepository;
        _planGenerator = planGenerator;
        _planScheduler = planScheduler;
        _logger = logger;
    }

    public async Task<BaseResponse<bool>> CreatePlanAsync(Guid userId, CreatePlanRequest request)
    {
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        if (userInfo == null)
            return BaseResponse<bool>.Fail(ErrorEnums.UserNotFound);

        if (userInfo.CurrentPlanId != null)
            return BaseResponse<bool>.Fail(ErrorEnums.PlanAlreadyExists);

        var generateRequest = MapToGenerateRequest(request, userInfo);

        var result = await _planGenerator.GeneratePlanAsync(generateRequest);
        if (!result.Success || result.Data == null)
            return BaseResponse<bool>.Fail(ErrorEnums.PlanGenerationFailed);

        var scheduled = await _planScheduler.ScheduleAsync(result.Data, userId, request.SessionDurationMinutes);

        var plan = MapToAIPlan(scheduled, request, userInfo);

        await _planRepository.AddAsync(plan);

        // CurrentWeekId нельзя ставить до сохранения — circular dependency с WeekPlan
        var firstWeek = plan.WeekPlans.OrderBy(w => w.NumberInPlan).FirstOrDefault();
        if (firstWeek != null)
        {
            plan.CurrentWeekId = firstWeek.Id;
            await _planRepository.UpdateAsync(plan);
        }

        userInfo.CurrentPlanId = plan.Id;
        await _userInfoRepository.UpdateAsync(userInfo);

        return BaseResponse<bool>.Ok(true);
    }

    public async Task<BaseResponse<PlanDto>> GetPlanAsync(Guid userId)
    {
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        if (userInfo == null)
            return BaseResponse<PlanDto>.Fail(ErrorEnums.UserNotFound);

        if (userInfo.CurrentPlanId == null)
            return BaseResponse<PlanDto>.Fail(ErrorEnums.PlanNotFound);

        var plan = await _planRepository.GetByIdWithDetailsAsync(userInfo.CurrentPlanId.Value);
        if (plan == null)
            return BaseResponse<PlanDto>.Fail(ErrorEnums.PlanNotFound);

        return BaseResponse<PlanDto>.Ok(MapToPlanDto(plan));
    }

    public async Task<BaseResponse<bool>> DeletePlanAsync(Guid userId)
    {
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        if (userInfo == null)
            return BaseResponse<bool>.Fail(ErrorEnums.UserNotFound);

        if (userInfo.CurrentPlanId == null)
            return BaseResponse<bool>.Fail(ErrorEnums.PlanNotFound);

        var plan = await _planRepository.GetByIdWithDetailsAsync(userInfo.CurrentPlanId.Value);
        if (plan == null)
            return BaseResponse<bool>.Fail(ErrorEnums.PlanNotFound);

        userInfo.CurrentPlanId = null;
        await _userInfoRepository.UpdateAsync(userInfo);
        await _planRepository.DeleteAsync(plan);

        return BaseResponse<bool>.Ok(true);
    }

    public async Task<BaseResponse<bool>> HasPlanAsync(Guid userId)
    {
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        if (userInfo == null)
            return BaseResponse<bool>.Fail(ErrorEnums.UserNotFound);

        return BaseResponse<bool>.Ok(userInfo.CurrentPlanId != null);
    }

    private static GeneratePlanRequest MapToGenerateRequest(CreatePlanRequest request, UserInfo userInfo)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - userInfo.DateOfBirth.Year;
        if (userInfo.DateOfBirth > today.AddYears(-age)) age--;

        return new GeneratePlanRequest
        {
            PlanType = request.PlanType,
            PlanSubType = request.PlanSubType,
            GoalMetadata = request.GoalMetadata,
            SessionsPerWeek = request.SessionsPerWeek,
            SessionDurationMinutes = request.SessionDurationMinutes,
            PlanDurationWeeks = request.PlanDurationWeeks,
            AvailableEquipment = request.AvailableEquipment,
            FreeTextWish = request.FreeTextWish,
            Sex = userInfo.Sex.ToString(),
            AgeYears = age,
            WeightKg = (double)userInfo.WeightKg,
            HeightCm = (int)userInfo.HeightCm,
            FitnessLevel = userInfo.Level.ToString()
        };
    }

    private static AIPlan MapToAIPlan(ScheduledPlanDto scheduled, CreatePlanRequest request, UserInfo userInfo)
    {
        if (!Enum.TryParse<PlanType>(request.PlanType, true, out var planType))
            planType = PlanType.Mix;
        if (!Enum.TryParse<PlanSubType>(request.PlanSubType, true, out var planSubType))
            planSubType = PlanSubType.Custom;

        var plan = new AIPlan
        {
            Id = Guid.NewGuid(),
            Status = PlanStatus.Active,
            Name = scheduled.Name,
            Description = scheduled.Description,
            CreatedAt = DateTime.UtcNow,
            PlanType = planType,
            PlanSubType = planSubType,
            GoalMetadata = request.GoalMetadata,
            SessionsPerWeek = request.SessionsPerWeek,
            SessionDurationMinutes = request.SessionDurationMinutes,
            PlanDurationWeeks = request.PlanDurationWeeks,
            AvailableEquipment = request.AvailableEquipment,
            FreeTextWish = request.FreeTextWish,
            WeeksDuration = scheduled.Weeks.Count,
            UserInfoId = userInfo.Id
        };

        plan.WeekPlans = scheduled.Weeks.Select(w =>
        {
            var weekPlan = new WeekPlan
            {
                Id = Guid.NewGuid(),
                NumberInPlan = w.WeekNumber,
                Description = w.Description,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                WeekStatus = w.WeekNumber == 1 ? WeekStatus.Active : WeekStatus.Future,
                AIPlanId = plan.Id
            };

            weekPlan.PlanTrainings = w.Trainings.Select(t =>
            {
                var trainingSet = BuildTrainingSet(t, userInfo.UserId);

                var planTraining = new PlanTraining
                {
                    Id = Guid.NewGuid(),
                    TrainingNumber = t.TrainingNumber,
                    Description = t.Description,
                    StartPlannedDate = t.StartPlannedDate,
                    EndPlannedDate = t.StartPlannedDate.AddMinutes(t.EstimatedDurationMinutes),
                    WeekPlanId = weekPlan.Id,
                    TrainingSet = trainingSet,
                    TrainingSetId = trainingSet.Id
                };

                trainingSet.PlanTrainingId = planTraining.Id;
                return planTraining;
            }).ToList();

            return weekPlan;
        }).ToList();

        return plan;
    }

    private static TrainingSet BuildTrainingSet(ScheduledTrainingDto training, Guid createdByUserId)
    {
        var trainingSet = new TrainingSet
        {
            Id = Guid.NewGuid(),
            Name = training.Name,
            Description = training.Description,
            CreatedAt = DateTime.UtcNow,
            SetAccessType = SetAccessType.Private,
            CreatedByUserId = createdByUserId
        };

        trainingSet.SetBlocks = training.Blocks.Select(b =>
        {
            var block = new SetBlock
            {
                Id = Guid.NewGuid(),
                Order = b.Order,
                Name = b.Name,
                SetsCount = b.SetsCount,
                RestBetweenSetsSeconds = b.RestBetweenSetsSeconds,
                RestAfterBlockSeconds = b.RestAfterBlockSeconds,
                TrainingSetId = trainingSet.Id
            };

            block.ExerciseEntries = b.Exercises.Select(e =>
            {
                var entry = new ExerciseEntry
                {
                    Id = Guid.NewGuid(),
                    Order = e.Order,
                    ExerciseId = e.ExerciseId,
                    ExerciseTypeId = e.ExerciseTypeId,
                    Reps = e.Reps,
                    DurationSeconds = e.DurationSeconds,
                    DistanceMeters = e.DistanceMeters,
                    WeightKg = e.WeightKg,
                    SpeedKmh = e.SpeedKmh,
                    RestAfterCurrentEntrySeconds = e.RestAfterCurrentEntrySeconds,
                    SetBlockId = block.Id
                };

                entry.Intervals = e.Intervals.Select(i => new ExerciseInterval
                {
                    Id = Guid.NewGuid(),
                    Order = i.Order,
                    Reps = i.Reps,
                    DurationSeconds = i.DurationSeconds,
                    DistanceMeters = i.DistanceMeters,
                    WeightKg = i.WeightKg,
                    SpeedKmh = i.SpeedKmh,
                    ExerciseEntryId = entry.Id
                }).ToList();

                return entry;
            }).ToList();

            return block;
        }).ToList();

        return trainingSet;
    }

    private static PlanDto MapToPlanDto(AIPlan plan) => new()
    {
        Id = plan.Id,
        Status = plan.Status.ToString(),
        Name = plan.Name,
        Description = plan.Description,
        WeeksDuration = plan.WeeksDuration,
        PlanType = plan.PlanType.ToString(),
        PlanSubType = plan.PlanSubType.ToString(),
        Weeks = plan.WeekPlans
            .OrderBy(w => w.NumberInPlan)
            .Select(w => new WeekPlanDto
            {
                Id = w.Id,
                WeekNumber = w.NumberInPlan,
                Description = w.Description,
                StartDate = w.StartDate.ToString("yyyy-MM-dd"),
                EndDate = w.EndDate.ToString("yyyy-MM-dd"),
                WeekStatus = w.WeekStatus.ToString(),
                PlanTrainings = w.PlanTrainings
                    .OrderBy(t => t.TrainingNumber)
                    .Select(t => new PlanTrainingDto
                    {
                        Id = t.Id,
                        TrainingNumber = t.TrainingNumber,
                        Description = t.Description,
                        StartPlannedDate = t.StartPlannedDate.ToString("o"),
                        TrainingSet = t.TrainingSet == null ? null : new TrainingSetSummaryDto
                        {
                            Id = t.TrainingSet.Id,
                            Name = t.TrainingSet.Name
                        }
                    }).ToList()
            }).ToList()
    };
}
