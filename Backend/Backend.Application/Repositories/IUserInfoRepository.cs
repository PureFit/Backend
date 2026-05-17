using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IUserInfoRepository
{
    Task<UserInfo?> GetByUserIdAsync(Guid userId);
    Task<UserInfo?> GetByUserIdWithStatsAsync(Guid userId);
    Task AddAsync(UserInfo userInfo);
    Task UpdateAsync(UserInfo userInfo);
}
