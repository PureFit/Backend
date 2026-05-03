using Backend.Application.Common;
using Backend.Application.DTOs.Profile;
using Backend.Application.Mappers;
using Backend.Application.Repositories;
using Backend.Application.Services;
using Backend.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ProfileService : IProfileService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IUserMetricLogRepository _metricLogRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IUserGoogleTokenRepository _googleTokenRepository;
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IUserInfoRepository userInfoRepository,
        IUserMetricLogRepository metricLogRepository,
        IAuthRepository authRepository,
        IUserGoogleTokenRepository googleTokenRepository,
        IGoogleOAuthService googleOAuthService,
        ILogger<ProfileService> logger)
    {
        _userInfoRepository = userInfoRepository;
        _metricLogRepository = metricLogRepository;
        _authRepository = authRepository;
        _googleTokenRepository = googleTokenRepository;
        _googleOAuthService = googleOAuthService;
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

    public async Task<BaseResponse<bool>> ConnectGoogleCalendarAsync(Guid userId, string serverAuthCode)
    {
        try
        {
            var tokenPair = await _googleOAuthService.ExchangeAuthCodeAsync(serverAuthCode);
            if (tokenPair == null)
                return BaseResponse<bool>.Fail(ErrorEnums.GoogleAuthFailed.ToString());

            await _googleTokenRepository.UpsertAsync(new UserGoogleToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccessToken = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                AccessTokenExpiresAt = tokenPair.ExpiresAt,
                ConnectedEmail = tokenPair.Email,
                ConnectedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Google Calendar connected for UserId: {UserId}", userId);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect Google Calendar for UserId: {UserId}", userId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<List<GoogleCalendarEventDto>>> GetGoogleCalendarEventsAsync(
        Guid userId, DateTime from, DateTime to)
    {
        try
        {
            var tokenRecord = await _googleTokenRepository.GetByUserIdAsync(userId);
            if (tokenRecord == null)
                return BaseResponse<List<GoogleCalendarEventDto>>.Fail(ErrorEnums.CalendarNotConnected.ToString());

            // Refresh token if expired
            if (tokenRecord.AccessTokenExpiresAt <= DateTime.UtcNow.AddMinutes(5))
            {
                var refreshed = await _googleOAuthService.RefreshAccessTokenAsync(tokenRecord.RefreshToken);
                if (refreshed == null)
                    return BaseResponse<List<GoogleCalendarEventDto>>.Fail(ErrorEnums.GoogleAuthFailed.ToString());

                tokenRecord.AccessToken = refreshed.Value.AccessToken;
                tokenRecord.AccessTokenExpiresAt = refreshed.Value.ExpiresAt;
                await _googleTokenRepository.UpsertAsync(tokenRecord);
            }

            var events = await _googleOAuthService.GetCalendarEventsAsync(tokenRecord.AccessToken, from, to);
            return BaseResponse<List<GoogleCalendarEventDto>>.Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Google Calendar events for UserId: {UserId}", userId);
            return BaseResponse<List<GoogleCalendarEventDto>>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<bool>> DisconnectGoogleCalendarAsync(Guid userId)
    {
        await _googleTokenRepository.DeleteByUserIdAsync(userId);
        return BaseResponse<bool>.Ok(true);
    }

    public async Task<BaseResponse<bool>> IsCalendarConnectedAsync(Guid userId)
    {
        var token = await _googleTokenRepository.GetByUserIdAsync(userId);
        return BaseResponse<bool>.Ok(token != null);
    }

    public async Task<BaseResponse<ProfileDto>> GetProfileAsync(Guid userId)
    {
        try
        {
            var user = await _authRepository.GetByIdAsync(userId);
            if (user == null)
                return BaseResponse<ProfileDto>.Fail("User not found");

            var info = await _userInfoRepository.GetByUserIdAsync(userId);
            if (info == null)
                return BaseResponse<ProfileDto>.Fail("Profile not found");

            return BaseResponse<ProfileDto>.Ok(new ProfileDto
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Sex = info.Sex.ToString(),
                FitnessLevel = info.Level.ToString(),
                WeightKg = info.WeightKg,
                HeightCm = (int)info.HeightCm,
                Age = info.Age
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get profile for UserId: {UserId}", userId);
            return BaseResponse<ProfileDto>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }
}
