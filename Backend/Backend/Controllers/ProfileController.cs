using Backend.Application.DTOs.Profile;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

    [HttpGet("has-profile")]
    public async Task<IActionResult> HasProfile()
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.HasProfileAsync(userId);
        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.CompleteProfileAsync(userId, request);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("weight")]
    public async Task<IActionResult> UpdateWeight([FromBody] UpdateWeightRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.UpdateWeightAsync(userId, request.WeightKg);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("weight-history")]
    public async Task<IActionResult> GetWeightHistory([FromQuery] int limit = 30)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.GetWeightHistoryAsync(userId, limit);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
