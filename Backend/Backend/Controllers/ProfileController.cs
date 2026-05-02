using Backend.Application.DTOs.Profile;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/profile")]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.CompleteProfileAsync(userId, request);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
