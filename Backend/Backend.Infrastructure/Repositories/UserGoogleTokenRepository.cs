using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class UserGoogleTokenRepository : IUserGoogleTokenRepository
{
    private readonly AppDbContext _db;

    public UserGoogleTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<UserGoogleToken?> GetByUserIdAsync(Guid userId) =>
        _db.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == userId);

    public async Task UpsertAsync(UserGoogleToken token)
    {
        var existing = await _db.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == token.UserId);
        if (existing == null)
        {
            await _db.UserGoogleTokens.AddAsync(token);
        }
        else
        {
            existing.AccessToken = token.AccessToken;
            existing.RefreshToken = token.RefreshToken;
            existing.AccessTokenExpiresAt = token.AccessTokenExpiresAt;
            existing.ConnectedEmail = token.ConnectedEmail;
            existing.ConnectedAt = token.ConnectedAt;
            _db.UserGoogleTokens.Update(existing);
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var token = await _db.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        if (token != null)
        {
            _db.UserGoogleTokens.Remove(token);
            await _db.SaveChangesAsync();
        }
    }
}
