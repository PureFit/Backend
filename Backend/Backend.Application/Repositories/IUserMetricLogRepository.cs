using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IUserMetricLogRepository
{
    Task AddAsync(UserMetricLog log);
    Task<List<UserMetricLog>> GetByUserAndMetricAsync(Guid userId, MetricType metric, int limit = 30);
}
