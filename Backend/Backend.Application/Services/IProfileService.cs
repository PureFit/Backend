using Backend.Application.Common;
using Backend.Application.DTOs.Profile;

namespace Backend.Application.Services;

public interface IProfileService
{
    Task<BaseResponse<bool>> CompleteProfileAsync(Guid userId, CompleteProfileRequest request);
    Task<BaseResponse<bool>> HasProfileAsync(Guid userId);
    Task<BaseResponse<bool>> UpdateWeightAsync(Guid userId, decimal weightKg);
    Task<BaseResponse<List<WeightEntryDto>>> GetWeightHistoryAsync(Guid userId, int limit = 30);
}
