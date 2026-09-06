using System.Security.Claims;
using System.Text.Json;
using Backend.Application.Repositories;
using Backend.Core.Enums;

namespace Backend.Middleware;

public class SubscriptionCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SubscriptionCheckMiddleware> _logger;

    private static readonly string[] ProtectedPrefixes =
    [
        "/api/plan/create",
        "/api/aichat"
    ];

    private static readonly string[] SkippedPrefixes =
    [
        "/api/auth",
        "/api/subscription",
        "/hubs",
        "/swagger"
    ];

    public SubscriptionCheckMiddleware(RequestDelegate next, ILogger<SubscriptionCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ISubscriptionRepository subscriptionRepository)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (SkippedPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var isProtected = ProtectedPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        if (!isProtected)
        {
            await _next(context);
            return;
        }

        var userIdClaim = context.User?.FindFirstValue(ClaimTypes.Sid);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await _next(context);
            return;
        }

        try
        {
            var subscription = await subscriptionRepository.GetByUserIdAsync(userId);

            if (subscription == null ||
                subscription.Status == SubscriptionStatus.Canceled ||
                subscription.Status == SubscriptionStatus.None)
            {
                await WritePaymentRequiredAsync(context, "SubscriptionRequired", "Active subscription required to access this feature");
                return;
            }

            if (subscription.Status == SubscriptionStatus.Trial)
            {
                var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (subscription.TrialEndsAt.HasValue && subscription.TrialEndsAt.Value < nowUnix)
                {
                    await WritePaymentRequiredAsync(context, "TrialExpired", "Trial period has expired");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking subscription for user {UserId}", userId);
            // On error allow through — do not block on infrastructure failure
        }

        await _next(context);
    }

    private static async Task WritePaymentRequiredAsync(HttpContext context, string error, string message)
    {
        context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error, message }));
    }
}
