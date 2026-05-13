using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IUserMetricLogRepository
{
    Task AddAsync(UserMetricLog log);
    Task AddRangeAsync(IEnumerable<UserMetricLog> logs);
    Task<List<UserMetricLog>> GetByUserAndMetricAsync(Guid userId, MetricType metric, int limit = 30);
}
