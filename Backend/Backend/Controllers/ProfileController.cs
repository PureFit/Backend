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

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.GetProfileAsync(userId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("calendar/connect")]
    public async Task<IActionResult> ConnectGoogleCalendar([FromBody] GoogleCalendarConnectRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.ConnectGoogleCalendarAsync(userId, request.ServerAuthCode);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpDelete("calendar/disconnect")]
    public async Task<IActionResult> DisconnectGoogleCalendar()
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.DisconnectGoogleCalendarAsync(userId);
        return Ok(result);
    }

    [HttpGet("calendar/status")]
    public async Task<IActionResult> CalendarStatus()
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.IsCalendarConnectedAsync(userId);
        return Ok(result);
    }

    [HttpGet("calendar/events")]
    public async Task<IActionResult> GetCalendarEvents(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var userId = GetUserIdFromClaims();
        var result = await _profileService.GetGoogleCalendarEventsAsync(userId, from, to);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
