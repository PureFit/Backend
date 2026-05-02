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

    public async Task AddAsync(UserInfo userInfo)
    {
        await _dbContext.UserInfos.AddAsync(userInfo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserInfo userInfo)
    {
        _dbContext.UserInfos.Update(userInfo);
        await _dbContext.SaveChangesAsync();
    }
}
