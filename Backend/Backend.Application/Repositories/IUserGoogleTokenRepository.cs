using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IUserGoogleTokenRepository
{
    Task<UserGoogleToken?> GetByUserIdAsync(Guid userId);
    Task UpsertAsync(UserGoogleToken token);
    Task DeleteByUserIdAsync(Guid userId);
}
