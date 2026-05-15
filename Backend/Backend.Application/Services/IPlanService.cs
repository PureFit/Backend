using Backend.Application.Common;
using Backend.Application.DTOs.Plan;

namespace Backend.Application.Services;

public interface IPlanService
{
    Task<BaseResponse<bool>> HasPlanAsync(Guid userId);
    Task<BaseResponse<PlanDto>> GetPlanAsync(Guid userId);
    Task<BaseResponse<bool>> CreatePlanAsync(Guid userId, CreatePlanRequest request);
    Task<BaseResponse<bool>> DeletePlanAsync(Guid userId);
}
