using Backend.Application.Common;
using Backend.Application.DTOs.Profile;
using Backend.Application.Mappers;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ProfileService : IProfileService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IUserMetricLogRepository _metricLogRepository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IUserInfoRepository userInfoRepository,
        IUserMetricLogRepository metricLogRepository,
        ILogger<ProfileService> logger)
    {
        _userInfoRepository = userInfoRepository;
        _metricLogRepository = metricLogRepository;
        _logger = logger;
    }

    public async Task<BaseResponse<bool>> HasProfileAsync(Guid userId)
    {
        var existing = await _userInfoRepository.GetByUserIdAsync(userId);
        return BaseResponse<bool>.Ok(existing != null);
    }

    public async Task<BaseResponse<bool>> UpdateWeightAsync(Guid userId, decimal weightKg)
    {
        try
        {
            var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
            if (userInfo != null)
            {
                userInfo.WeightKg = weightKg;
                await _userInfoRepository.UpdateAsync(userInfo);
            }

            await _metricLogRepository.AddAsync(new UserMetricLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Metric = MetricType.Weight,
                Value = weightKg.ToString("F1"),
                LoggedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Weight updated for UserId: {UserId} → {Weight}kg", userId, weightKg);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update weight for UserId: {UserId}", userId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<WeightEntryDto>>> GetWeightHistoryAsync(Guid userId, int limit = 30)
    {
        try
        {
            var logs = await _metricLogRepository.GetByUserAndMetricAsync(userId, MetricType.Weight, limit);
            var result = logs
                .Select(l => new WeightEntryDto
                {
                    WeightKg = decimal.Parse(l.Value),
                    LoggedAt = l.LoggedAt
                })
                .OrderBy(e => e.LoggedAt)
                .ToList();

            return BaseResponse<List<WeightEntryDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weight history for UserId: {UserId}", userId);
            return BaseResponse<List<WeightEntryDto>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<bool>> CompleteProfileAsync(Guid userId, CompleteProfileRequest request)
    {
        try
        {
            var existing = await _userInfoRepository.GetByUserIdAsync(userId);

            if (existing != null)
            {
                request.UpdateEntity(existing);
                await _userInfoRepository.UpdateAsync(existing);
            }
            else
            {
                await _userInfoRepository.AddAsync(request.ToEntity(userId));
            }

            _logger.LogInformation("Profile completed for UserId: {UserId}", userId);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete profile for UserId: {UserId}", userId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }
}
