using Backend.Application.Common;
using Backend.Application.DTOs.Plan;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/plan")]
public class PlanController : BaseController
{
    private readonly IPlanService _planService;
    private readonly IServiceScopeFactory _scopeFactory;

    public PlanController(IPlanService planService, IServiceScopeFactory scopeFactory)
    {
        _planService = planService;
        _scopeFactory = scopeFactory;
    }

    [HttpGet("exists")]
    public async Task<IActionResult> HasPlan()
    {
        var userId = GetUserIdFromClaims();
        var result = await _planService.HasPlanAsync(userId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlan()
    {
        var userId = GetUserIdFromClaims();
        var result = await _planService.GetPlanAsync(userId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
    {
        var userId = GetUserIdFromClaims();

        // Быстрая валидация — план уже есть?
        var check = await _planService.HasPlanAsync(userId);
        if (!check.Success) return HandleError(check);
        if (check.Data == true) return Conflict(BaseResponse<bool>.Fail(ErrorEnums.PlanAlreadyExists));

        // Запускаем генерацию в фоне с отдельным DI-scope
        _ = Task.Run(async () =>
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var svc = scope.ServiceProvider.GetRequiredService<IPlanService>();
            await svc.CreatePlanAsync(userId, request);
        });

        return Accepted(BaseResponse<bool>.Ok(true));
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlan()
    {
        var userId = GetUserIdFromClaims();
        var result = await _planService.DeletePlanAsync(userId);
        return result.Success ? Ok(result) : HandleError(result);
    }
}
