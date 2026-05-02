using Backend.Application.Common;
using Backend.Application.DTOs.Profile;
using Backend.Application.Mappers;
using Backend.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ProfileService : IProfileService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IUserInfoRepository userInfoRepository, ILogger<ProfileService> logger)
    {
        _userInfoRepository = userInfoRepository;
        _logger = logger;
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
