using Backend.Application.Common.Exceptions;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private AppDbContext _dbContext;
    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken refreshToken)
    {
        var existingToken = await _dbContext.RefreshTokens.FindAsync(refreshToken.Id);
        if (existingToken == null)
            throw new NotFoundInDbException();

        _dbContext.RefreshTokens.Remove(existingToken);
    }
}
