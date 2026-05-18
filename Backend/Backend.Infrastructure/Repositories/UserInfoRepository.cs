using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class UserInfoRepository : IUserInfoRepository
{
    private readonly AppDbContext _dbContext;

    public UserInfoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserInfo?> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<UserInfo?> GetByUserIdWithStatsAsync(Guid userId)
    {
        return await _dbContext.UserInfos
            .Include(u => u.WorkloadStats)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(UserInfo userInfo)
    {
        await _dbContext.UserInfos.AddAsync(userInfo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserInfo userInfo)
    {
        if (_dbContext.Entry(userInfo).State == EntityState.Detached)
            _dbContext.UserInfos.Update(userInfo);

        await _dbContext.SaveChangesAsync();
    }
}
