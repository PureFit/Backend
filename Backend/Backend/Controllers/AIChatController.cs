using Backend.Application.Common;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/ai/chat")]
public class AIChatController : BaseController
{
    private readonly IAIChatService _chatService;

    public AIChatController(IAIChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] AIChatRequest request)
    {
        var result = await _chatService.ChatAsync(GetUserIdFromClaims(), request);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory()
    {
        await _chatService.ClearHistoryAsync(GetUserIdFromClaims());
        return Ok(BaseResponse<bool>.Ok(true));
    }

    [HttpGet("usage")]
    public async Task<IActionResult> GetUsage()
    {
        var usage = await _chatService.GetUsageAsync(GetUserIdFromClaims());
        return Ok(BaseResponse<UserUsageDto>.Ok(usage));
    }
}
