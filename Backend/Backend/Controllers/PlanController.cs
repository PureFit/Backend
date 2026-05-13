using Backend.Application.Common;
using Backend.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/plan")]
public class PlanController : BaseController
{
    private readonly IUserInfoRepository _userInfoRepository;

    public PlanController(IUserInfoRepository userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }

    [HttpGet("exists")]
    public async Task<IActionResult> HasPlan()
    {
        var userId = GetUserIdFromClaims();
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        var hasPlan = userInfo?.CurrentPlanId != null;
        return Ok(BaseResponse<bool>.Ok(hasPlan));
    }
}
