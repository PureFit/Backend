using Backend.Application.Common;
using Backend.Application.DTOs.Session;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class TrainingSessionService : ITrainingSessionService
{
    private readonly ITrainingSessionRepository _sessionRepo;
    private readonly ITrainingSetRepository _setRepo;
    private readonly IUserInfoRepository _userInfoRepo;
    private readonly IMuscleCalculatorService _muscleCalculator;
    private readonly ILogger<TrainingSessionService> _logger;

    public TrainingSessionService(
        ITrainingSessionRepository sessionRepo,
        ITrainingSetRepository setRepo,
        IUserInfoRepository userInfoRepo,
        IMuscleCalculatorService muscleCalculator,
        ILogger<TrainingSessionService> logger)
    {
        _sessionRepo = sessionRepo;
        _setRepo = setRepo;
        _userInfoRepo = userInfoRepo;
        _muscleCalculator = muscleCalculator;
        _logger = logger;
    }

    public async Task<BaseResponse<TrainingSessionDto>> StartSessionAsync(Guid userId, StartSessionRequest request)
    {
        try
        {
            var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
            if (userInfo is null)
            {
                _logger.LogWarning("UserInfo not found for user {UserId}", userId);
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UserNotFound);
            }

            var set = await _setRepo.GetByIdAsync(request.TrainingSetId);
            if (set is null)
            {
                _logger.LogWarning("TrainingSet {SetId} not found", request.TrainingSetId);
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.NotFound);
            }

            if (set.SetAccessType == SetAccessType.Private &&
                set.CreatedByUserId != null &&
                set.CreatedByUserId != userId)
            {
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.Forbidden);
            }

            var session = new TrainingSession
            {
                Id = Guid.NewGuid(),
                UserInfoId = userInfo.Id,
                TrainingSetId = request.TrainingSetId,
                PlanTrainingId = request.PlanTrainingId,
                Status = SessionStatus.InProgress,
                Start = DateTime.UtcNow
            };

            await _sessionRepo.CreateAsync(session);
            _logger.LogInformation("Session {SessionId} started for user {UserId}", session.Id, userId);

            return BaseResponse<TrainingSessionDto>.Ok(ToDto(session, set.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting session for user {UserId}", userId);
            return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<TrainingSessionDto>> FinishSessionAsync(Guid userId, Guid sessionId)
    {
        try
        {
            var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
            if (userInfo is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UserNotFound);

            var session = await _sessionRepo.GetByIdAsync(sessionId, userInfo.Id);
            if (session is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.SessionNotFound);

            if (session.Status != SessionStatus.InProgress)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.ValidationError);

            session.Status = SessionStatus.Completed;
            session.End = DateTime.UtcNow;

            await _sessionRepo.UpdateAsync(session);
            _logger.LogInformation("Session {SessionId} completed for user {UserId}", sessionId, userId);

            await UpdateWorkloadStatsAsync(userId, session.TrainingSetId);

            var setName = session.TrainingSet?.Name ?? "";
            return BaseResponse<TrainingSessionDto>.Ok(ToDto(session, setName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finishing session {SessionId}", sessionId);
            return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<TrainingSessionDto>> AbandonSessionAsync(Guid userId, Guid sessionId)
    {
        try
        {
            var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
            if (userInfo is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UserNotFound);

            var session = await _sessionRepo.GetByIdAsync(sessionId, userInfo.Id);
            if (session is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.SessionNotFound);

            if (session.Status != SessionStatus.InProgress)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.ValidationError);

            session.Status = SessionStatus.Abandoned;
            session.End = DateTime.UtcNow;

            await _sessionRepo.UpdateAsync(session);
            _logger.LogInformation("Session {SessionId} abandoned for user {UserId}", sessionId, userId);

            var setName = session.TrainingSet?.Name ?? "";
            return BaseResponse<TrainingSessionDto>.Ok(ToDto(session, setName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error abandoning session {SessionId}", sessionId);
            return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<TrainingSessionDto>> GetByIdAsync(Guid userId, Guid sessionId)
    {
        try
        {
            var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
            if (userInfo is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UserNotFound);

            var session = await _sessionRepo.GetByIdAsync(sessionId, userInfo.Id);
            if (session is null)
                return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.SessionNotFound);

            return BaseResponse<TrainingSessionDto>.Ok(ToDto(session, session.TrainingSet?.Name ?? ""));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session {SessionId}", sessionId);
            return BaseResponse<TrainingSessionDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<SessionHistoryResponse>> GetHistoryAsync(Guid userId, int page, int pageSize)
    {
        try
        {
            var userInfo = await _userInfoRepo.GetByUserIdAsync(userId);
            if (userInfo is null)
                return BaseResponse<SessionHistoryResponse>.Fail(ErrorEnums.UserNotFound);

            var (items, total) = await _sessionRepo.GetHistoryAsync(userInfo.Id, page, pageSize);

            var dtos = items.Select(s => ToDto(s, s.TrainingSet?.Name ?? "")).ToList();

            return BaseResponse<SessionHistoryResponse>.Ok(new SessionHistoryResponse
            {
                Items = dtos,
                TotalCount = total
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history for user {UserId}", userId);
            return BaseResponse<SessionHistoryResponse>.Fail(ErrorEnums.UnknownError);
        }
    }

    private async Task UpdateWorkloadStatsAsync(Guid userId, Guid? trainingSetId)
    {
        if (trainingSetId is null) return;

        var userInfo = await _userInfoRepo.GetByUserIdWithStatsAsync(userId);
        if (userInfo is null) return;

        var result = await _muscleCalculator.CalculateRawVolumeForSetAsync(trainingSetId.Value, (float)userInfo.WeightKg);
        if (!result.Success) return;

        var now = DateTime.UtcNow;

        foreach (var (name, vol) in result.Data.MuscleVolumes)
            UpsertStat(userInfo, name, WorkloadStatCategory.Muscle, vol, now);

        foreach (var (name, vol) in result.Data.BodyPartVolumes)
            UpsertStat(userInfo, name, WorkloadStatCategory.BodyPart, vol, now);

        await _userInfoRepo.UpdateAsync(userInfo);
    }

    private static void UpsertStat(UserInfo userInfo, string name, WorkloadStatCategory category, float volume, DateTime now)
    {
        var stat = userInfo.WorkloadStats.FirstOrDefault(s => s.Name == name && s.Category == category);
        if (stat is null)
        {
            userInfo.WorkloadStats.Add(new UserWorkloadStat
            {
                Id = Guid.NewGuid(),
                UserInfoId = userInfo.Id,
                Name = name,
                Category = category,
                AccumulatedVolume = volume,
                SessionCount = 1,
                LastUpdatedAt = now
            });
        }
        else
        {
            stat.AccumulatedVolume += volume;
            stat.SessionCount++;
            stat.LastUpdatedAt = now;
        }
    }

    private static TrainingSessionDto ToDto(TrainingSession s, string setName) => new()
    {
        Id = s.Id,
        Status = s.Status,
        Start = s.Start,
        End = s.End,
        DurationMinutes = s.End.HasValue ? (int)(s.End.Value - s.Start).TotalMinutes : null,
        TrainingSetId = s.TrainingSetId ?? Guid.Empty,
        TrainingSetName = setName,
        PlanTrainingId = s.PlanTrainingId
    };
}
