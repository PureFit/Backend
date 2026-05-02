using Backend.Application.Common;
using Backend.Application.DTOs.Profile;

namespace Backend.Application.Services;

public interface IProfileService
{
    Task<BaseResponse<bool>> CompleteProfileAsync(Guid userId, CompleteProfileRequest request);
}
