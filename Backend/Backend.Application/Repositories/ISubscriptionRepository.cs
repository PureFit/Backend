using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface ISubscriptionRepository
{
    Task<UserSubscription?> GetByUserIdAsync(Guid userId);
    Task<UserSubscription?> GetByStripeCustomerIdOrSubscriptionIdAsync(string? customerId, string? subscriptionId);
    Task UpsertAsync(UserSubscription subscription);
}
