using Backend.Application.Common;
using Backend.Application.DTOs.Subscription;

namespace Backend.Application.Services;

public interface ISubscriptionService
{
    Task<BaseResponse<SubscriptionStatusDto>> GetStatusAsync(Guid userId);
    Task<BaseResponse<CheckoutSessionResponse>> CreateCheckoutSessionAsync(Guid userId, CreateCheckoutSessionRequest request);
    Task<BaseResponse<CheckoutSessionResponse>> CreatePortalSessionAsync(Guid userId, string returnUrl);
    Task<BaseResponse<bool>> HandleWebhookAsync(string payload, string stripeSignature);
}
