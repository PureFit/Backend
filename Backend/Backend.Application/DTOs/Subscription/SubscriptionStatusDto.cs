namespace Backend.Application.DTOs.Subscription;

public class SubscriptionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public long? CurrentPeriodEnd { get; set; }
    public long? TrialEndsAt { get; set; }
    public string? PriceId { get; set; }
    public string? StripeCustomerId { get; set; }
}
