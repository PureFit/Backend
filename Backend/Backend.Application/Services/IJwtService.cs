using Backend.Application.DTOs.Auth;
using Backend.Core.Entities;

namespace Backend.Application.Services;

public interface IJwtService
{
    Task<AuthTokenDto> GenerateToken(User user);
    Task<Guid?> ValidateRefreshTokenAsync(string refreshToken);
}
