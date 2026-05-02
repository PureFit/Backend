using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task DeleteAsync(RefreshToken refreshToken);
}
