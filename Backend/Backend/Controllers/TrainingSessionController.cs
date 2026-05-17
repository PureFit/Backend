using Backend.Application.DTOs.Session;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/sessions")]
public class TrainingSessionController : BaseController
{
    private readonly ITrainingSessionService _sessionService;

    public TrainingSessionController(ITrainingSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>Start a new training session (Status = InProgress)</summary>
    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartSessionRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _sessionService.StartSessionAsync(userId, request);
        return result.Success ? Ok(result) : HandleError(result);
    }

    /// <summary>Mark session as Completed</summary>
    [HttpPost("{sessionId:guid}/finish")]
    public async Task<IActionResult> Finish(Guid sessionId)
    {
        var userId = GetUserIdFromClaims();
        var result = await _sessionService.FinishSessionAsync(userId, sessionId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    /// <summary>Mark session as Abandoned</summary>
    [HttpPost("{sessionId:guid}/abandon")]
    public async Task<IActionResult> Abandon(Guid sessionId)
    {
        var userId = GetUserIdFromClaims();
        var result = await _sessionService.AbandonSessionAsync(userId, sessionId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetById(Guid sessionId)
    {
        var userId = GetUserIdFromClaims();
        var result = await _sessionService.GetByIdAsync(userId, sessionId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    /// <summary>Get paginated session history for current user</summary>
    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserIdFromClaims();
        var result = await _sessionService.GetHistoryAsync(userId, page, pageSize);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
