using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[AllowAnonymous]
[Route("subscription")]
public class SubscriptionRedirectController : Controller
{
    [HttpGet("success")]
    public ContentResult Success()
    {
        var html = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Payment Successful</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { background: #0f0f0f; color: #fff; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                    .card { text-align: center; padding: 48px 32px; max-width: 360px; }
                    .icon { font-size: 64px; margin-bottom: 24px; }
                    h1 { font-size: 28px; font-weight: 800; margin-bottom: 12px; }
                    .accent { color: #c6f135; }
                    p { color: #888; font-size: 15px; line-height: 1.6; margin-bottom: 32px; }
                    .btn { display: inline-block; background: #c6f135; color: #0f0f0f; font-weight: 700; font-size: 15px; padding: 14px 32px; border-radius: 12px; text-decoration: none; }
                </style>
            </head>
            <body>
                <div class="card">
                    <div class="icon">✅</div>
                    <h1>You're <span class="accent">Pro</span> now!</h1>
                    <p>Your subscription is active. Return to PureFit and enjoy unlimited AI training.</p>
                    <a href="purefit://subscription/success" class="btn">Back to App</a>
                </div>
            </body>
            </html>
            """;
        return Content(html, "text/html");
    }

    [HttpGet("cancel")]
    public ContentResult Cancel()
    {
        var html = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Payment Cancelled</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { background: #0f0f0f; color: #fff; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; display: flex; align-items: center; justify-content: center; min-height: 100vh; }
                    .card { text-align: center; padding: 48px 32px; max-width: 360px; }
                    .icon { font-size: 64px; margin-bottom: 24px; }
                    h1 { font-size: 28px; font-weight: 800; margin-bottom: 12px; }
                    p { color: #888; font-size: 15px; line-height: 1.6; margin-bottom: 32px; }
                    .btn { display: inline-block; background: #c6f135; color: #0f0f0f; font-weight: 700; font-size: 15px; padding: 14px 32px; border-radius: 12px; text-decoration: none; }
                </style>
            </head>
            <body>
                <div class="card">
                    <div class="icon">↩️</div>
                    <h1>Payment cancelled</h1>
                    <p>No worries — you can subscribe anytime from the PureFit app.</p>
                    <a href="purefit://subscription/cancel" class="btn">Back to App</a>
                </div>
            </body>
            </html>
            """;
        return Content(html, "text/html");
    }
}
