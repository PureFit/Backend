using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/achievements")]
public class AchievementController : BaseController
{
    private readonly IAchievementService _achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = GetUserIdFromClaims();
        var result = await _achievementService.GetUserAchievementsAsync(userId);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
