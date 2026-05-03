using Backend.Application.Common;
using Backend.Application.DTOs.Auth;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Backend.Controllers;

[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : HandleAuthError(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return result.Success ? Ok(result) : HandleAuthError(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshAsync(request);
        return result.Success ? Ok(result) : HandleAuthError(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
    {
        var result = await _authService.GoogleLoginAsync(request.IdToken);
        return result.Success ? Ok(result) : HandleAuthError(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);
        return result.Success ? Ok(result) : HandleAuthError(result);
    }

    private IActionResult HandleAuthError<T>(BaseResponse<T> result) =>
        result.Message switch
        {
            nameof(ErrorEnums.EmailExists) => Conflict(result),
            nameof(ErrorEnums.UsernameExists) => Conflict(result),
            nameof(ErrorEnums.PasswordsMismatch) => BadRequest(result),
            nameof(ErrorEnums.InvalidCredentials) => Unauthorized(result),
            nameof(ErrorEnums.InvalidRefreshToken) => Unauthorized(result),
            nameof(ErrorEnums.WrongAuthProvider) => BadRequest(result),
            nameof(ErrorEnums.GoogleAuthFailed) => Unauthorized(result),
            nameof(ErrorEnums.ImageSaveError) => StatusCode(500, result),
            _ => HandleError(result)
        };
}
