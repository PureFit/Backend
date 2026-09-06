using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserSubscription?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<UserSubscription?> GetByStripeCustomerIdOrSubscriptionIdAsync(string? customerId, string? subscriptionId)
    {
        return await _context.UserSubscriptions
            .FirstOrDefaultAsync(s =>
                (!string.IsNullOrEmpty(customerId) && s.StripeCustomerId == customerId) ||
                (!string.IsNullOrEmpty(subscriptionId) && s.StripeSubscriptionId == subscriptionId));
    }

    public async Task UpsertAsync(UserSubscription subscription)
    {
        var existing = await _context.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == subscription.UserId);

        if (existing == null)
        {
            await _context.UserSubscriptions.AddAsync(subscription);
        }
        else
        {
            existing.StripeCustomerId = subscription.StripeCustomerId;
            existing.StripeSubscriptionId = subscription.StripeSubscriptionId;
            existing.Status = subscription.Status;
            existing.PriceId = subscription.PriceId;
            existing.TrialEndsAt = subscription.TrialEndsAt;
            existing.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
            _context.UserSubscriptions.Update(existing);
        }

        await _context.SaveChangesAsync();
    }
}
