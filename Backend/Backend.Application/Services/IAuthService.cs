using Backend.Application.Common;
using Backend.Application.DTOs.Auth;

namespace Backend.Application.Services;

public interface IAuthService
{
    Task<BaseResponse<AuthTokenDto>> RegisterAsync(RegisterRequest request);
    Task<BaseResponse<AuthTokenDto>> LoginAsync(LoginRequest request);
    Task<BaseResponse<AuthTokenDto>> RefreshAsync(RefreshRequest request);
    Task<BaseResponse<bool>> LogoutAsync(string refreshToken);
}
