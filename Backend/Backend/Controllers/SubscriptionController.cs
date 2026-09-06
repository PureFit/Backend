using Backend.Application.DTOs.Subscription;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[Route("api/subscription")]
public class SubscriptionController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var userId = GetUserIdFromClaims();
        var result = await _subscriptionService.GetStatusAsync(userId);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutSessionRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _subscriptionService.CreateCheckoutSessionAsync(userId, request);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [HttpPost("portal")]
    public async Task<IActionResult> CreatePortal([FromBody] PortalRequest request)
    {
        var userId = GetUserIdFromClaims();
        var result = await _subscriptionService.CreatePortalSessionAsync(userId, request.ReturnUrl);
        return result.Success ? Ok(result) : HandleError(result);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? string.Empty;
        var result = await _subscriptionService.HandleWebhookAsync(payload, stripeSignature);
        return result.Success ? Ok(result) : HandleError(result);
    }
}

public class PortalRequest
{
    public string ReturnUrl { get; set; } = string.Empty;
}
