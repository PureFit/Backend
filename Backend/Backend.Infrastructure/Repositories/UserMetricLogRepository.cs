using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class UserMetricLogRepository : IUserMetricLogRepository
{
    private readonly AppDbContext _dbContext;

    public UserMetricLogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(UserMetricLog log)
    {
        await _dbContext.UserMetricLogs.AddAsync(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<UserMetricLog>> GetByUserAndMetricAsync(Guid userId, MetricType metric, int limit = 30)
    {
        return await _dbContext.UserMetricLogs
            .Where(l => l.UserId == userId && l.Metric == metric)
            .OrderByDescending(l => l.LoggedAt)
            .Take(limit)
            .ToListAsync();
    }
}
