using Backend.Application.Common;
using Backend.Application.DTOs.Subscription;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Backend.Application.Services.impl;

public class SubscriptionService : ISubscriptionService
{
    private readonly ILogger<SubscriptionService> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly StripeSettings _stripeSettings;
    private readonly StripeClient _stripeClient;

    public SubscriptionService(
        ILogger<SubscriptionService> logger,
        ISubscriptionRepository subscriptionRepository,
        IOptions<StripeSettings> stripeOptions)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _stripeSettings = stripeOptions.Value;
        _stripeClient = new StripeClient(_stripeSettings.SecretKey);
    }

    public async Task<BaseResponse<SubscriptionStatusDto>> GetStatusAsync(Guid userId)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);

            var dto = subscription == null
                ? new SubscriptionStatusDto { Status = SubscriptionStatus.None.ToString() }
                : new SubscriptionStatusDto
                {
                    Status = subscription.Status.ToString(),
                    CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                    TrialEndsAt = subscription.TrialEndsAt,
                    PriceId = subscription.PriceId,
                    StripeCustomerId = subscription.StripeCustomerId
                };

            return BaseResponse<SubscriptionStatusDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscription status for user {UserId}", userId);
            return BaseResponse<SubscriptionStatusDto>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<CheckoutSessionResponse>> CreateCheckoutSessionAsync(Guid userId, CreateCheckoutSessionRequest request)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByUserIdAsync(userId)
                ?? new UserSubscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Status = SubscriptionStatus.None,
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

            string customerId = subscription.StripeCustomerId ?? string.Empty;

            if (string.IsNullOrEmpty(customerId))
            {
                var customerService = new CustomerService(_stripeClient);
                var customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId.ToString() }
                    }
                });
                customerId = customer.Id;
                subscription.StripeCustomerId = customerId;
                await _subscriptionRepository.UpsertAsync(subscription);
            }

            var sessionService = new Stripe.Checkout.SessionService(_stripeClient);
            var sessionOptions = new Stripe.Checkout.SessionCreateOptions
            {
                Customer = customerId,
                Mode = "subscription",
                LineItems =
                [
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        Price = request.PriceId,
                        Quantity = 1
                    }
                ],
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl
            };

            var session = await sessionService.CreateAsync(sessionOptions);

            return BaseResponse<CheckoutSessionResponse>.Ok(new CheckoutSessionResponse { Url = session.Url });
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating checkout session for user {UserId}", userId);
            return BaseResponse<CheckoutSessionResponse>.Fail(ErrorEnums.UnknownError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session for user {UserId}", userId);
            return BaseResponse<CheckoutSessionResponse>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<CheckoutSessionResponse>> CreatePortalSessionAsync(Guid userId, string returnUrl)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);

            if (subscription == null || string.IsNullOrEmpty(subscription.StripeCustomerId))
                return BaseResponse<CheckoutSessionResponse>.Fail(ErrorEnums.NotFound);

            var portalService = new Stripe.BillingPortal.SessionService(_stripeClient);
            var portalOptions = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = subscription.StripeCustomerId,
                ReturnUrl = returnUrl
            };

            var portalSession = await portalService.CreateAsync(portalOptions);

            return BaseResponse<CheckoutSessionResponse>.Ok(new CheckoutSessionResponse { Url = portalSession.Url });
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating portal session for user {UserId}", userId);
            return BaseResponse<CheckoutSessionResponse>.Fail(ErrorEnums.UnknownError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating portal session for user {UserId}", userId);
            return BaseResponse<CheckoutSessionResponse>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> HandleWebhookAsync(string payload, string stripeSignature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, _stripeSettings.WebhookSecret);

            _logger.LogInformation("Handling Stripe webhook event: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "customer.subscription.created":
                case "customer.subscription.updated":
                    await HandleSubscriptionUpdatedAsync(stripeEvent);
                    break;

                case "customer.subscription.deleted":
                    await HandleSubscriptionDeletedAsync(stripeEvent);
                    break;

                case "invoice.payment_failed":
                    await HandleInvoicePaymentFailedAsync(stripeEvent);
                    break;

                case "checkout.session.completed":
                    await HandleCheckoutSessionCompletedAsync(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return BaseResponse<bool>.Ok(true);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Invalid Stripe webhook signature or payload");
            return BaseResponse<bool>.Fail(ErrorEnums.ValidationError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Stripe webhook");
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    private async Task HandleSubscriptionUpdatedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Stripe.Subscription stripeSubscription)
            return;

        var subscription = await _subscriptionRepository.GetByStripeCustomerIdOrSubscriptionIdAsync(
            stripeSubscription.CustomerId, stripeSubscription.Id);

        if (subscription == null)
        {
            _logger.LogWarning("No user subscription found for Stripe customer {CustomerId} or subscription {SubId}",
                stripeSubscription.CustomerId, stripeSubscription.Id);
            return;
        }

        subscription.StripeSubscriptionId = stripeSubscription.Id;
        subscription.Status = MapStripeStatus(stripeSubscription.Status);
        subscription.PriceId = stripeSubscription.Items?.Data?.FirstOrDefault()?.Price?.Id;

        var firstItem = stripeSubscription.Items?.Data?.FirstOrDefault();
        if (firstItem != null && firstItem.CurrentPeriodEnd != default)
            subscription.CurrentPeriodEnd = ((DateTimeOffset)firstItem.CurrentPeriodEnd).ToUnixTimeSeconds();

        if (stripeSubscription.TrialEnd.HasValue)
            subscription.TrialEndsAt = ((DateTimeOffset)stripeSubscription.TrialEnd.Value).ToUnixTimeSeconds();

        await _subscriptionRepository.UpsertAsync(subscription);
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Stripe.Subscription stripeSubscription)
            return;

        var subscription = await _subscriptionRepository.GetByStripeCustomerIdOrSubscriptionIdAsync(
            stripeSubscription.CustomerId, stripeSubscription.Id);

        if (subscription == null)
        {
            _logger.LogWarning("No user subscription found for deleted Stripe subscription {SubId}", stripeSubscription.Id);
            return;
        }

        subscription.Status = SubscriptionStatus.Canceled;
        await _subscriptionRepository.UpsertAsync(subscription);
    }

    private async Task HandleInvoicePaymentFailedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Invoice invoice)
            return;

        var subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId;

        var subscription = await _subscriptionRepository.GetByStripeCustomerIdOrSubscriptionIdAsync(
            invoice.CustomerId, subscriptionId);

        if (subscription == null)
        {
            _logger.LogWarning("No user subscription found for failed invoice customer {CustomerId}", invoice.CustomerId);
            return;
        }

        subscription.Status = SubscriptionStatus.PastDue;
        await _subscriptionRepository.UpsertAsync(subscription);
    }

    private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Stripe.Checkout.Session session)
            return;

        if (session.Mode != "subscription" || string.IsNullOrEmpty(session.CustomerId))
            return;

        var subscription = await _subscriptionRepository.GetByStripeCustomerIdOrSubscriptionIdAsync(
            session.CustomerId, null);

        if (subscription == null)
        {
            _logger.LogWarning("No user subscription found for checkout session customer {CustomerId}", session.CustomerId);
            return;
        }

        if (!string.IsNullOrEmpty(session.SubscriptionId))
            subscription.StripeSubscriptionId = session.SubscriptionId;

        subscription.StripeCustomerId = session.CustomerId;
        await _subscriptionRepository.UpsertAsync(subscription);
    }

    private static SubscriptionStatus MapStripeStatus(string? status) => status switch
    {
        "trialing" => SubscriptionStatus.Trial,
        "active" => SubscriptionStatus.Active,
        "past_due" => SubscriptionStatus.PastDue,
        "canceled" or "cancelled" or "unpaid" or "incomplete_expired" => SubscriptionStatus.Canceled,
        _ => SubscriptionStatus.None
    };
}
