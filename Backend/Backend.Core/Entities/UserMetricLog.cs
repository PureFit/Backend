namespace Backend.Core.Entities;

public class UserMetricLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public MetricType Metric { get; set; }
    public string Value { get; set; } = null!;
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}

public enum MetricType
{
    Weight,
    Height,
    FitnessLevel
}
