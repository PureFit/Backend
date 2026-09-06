using Backend.Core.Enums;

namespace Backend.Core.Entities;

public class UserSubscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public string? PriceId { get; set; }
    public long? TrialEndsAt { get; set; }
    public long? CurrentPeriodEnd { get; set; }
    public long CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
