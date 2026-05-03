using Backend.Application.Common;
using Backend.Application.Common.Exceptions;
using Backend.Application.DTOs.Auth;
using Backend.Application.Helpers.impl;
using Backend.Application.Repositories;
using Backend.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace Backend.Application.Services.impl;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IImageOrchestratorService _imageService;
    private readonly IJwtService _jwtService;

    public AuthService(
        ILogger<AuthService> logger,
        IAuthRepository authRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserInfoRepository userInfoRepository,
        IOptions<JwtSettings> options,
        IImageOrchestratorService imageService,
        IJwtService jwtService)
    {
        _authRepository = authRepository ?? throw new ArgumentNullException(nameof(IAuthRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(IRefreshTokenRepository));
        _userInfoRepository = userInfoRepository ?? throw new ArgumentNullException(nameof(IUserInfoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<AuthService>));
        _jwtSettings = options.Value ?? throw new ArgumentNullException(nameof(IOptions<JwtSettings>));
        _imageService = imageService ?? throw new ArgumentNullException(nameof(IImageOrchestratorService));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(IJwtService));
    }

    private async Task<bool> EmailExists(string email) =>
        await _authRepository.ExistsByEmailAsync(email);

    private async Task<bool> UsernameExists(string username) =>
        await _authRepository.ExistsByUsernameAsync(username);

    public async Task<BaseResponse<AuthTokenDto>> RegisterAsync(RegisterRequest request)
    {
        var userId = Guid.NewGuid();

        try
        {
            if (request.Password != request.PasswordConfirm)
            {
                _logger.LogWarning("Password and confirmation do not match for user {Username}", request.Username);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.PasswordsMismatch.ToString());
            }

            if (await EmailExists(request.Email))
            {
                _logger.LogWarning("Email {Email} already exists", request.Email);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.EmailExists.ToString());
            }

            if (await UsernameExists(request.Username))
            {
                _logger.LogWarning("Username {Username} already exists", request.Username);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.UsernameExists.ToString());
            }

            _logger.LogInformation("Registering new user {Username} with email {Email}. AssignedId: {UserId}", request.Username, request.Email, userId);
            var imageUrl = request.AvatarImg is not null
                ? await _imageService.UploadFileAsync(request.AvatarImg, userId)
                : null;

            var user = new User
            {
                Id = userId,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password.HashPassword(),
                AvatarUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
            };

            await _authRepository.AddAsync(user);

            _logger.LogInformation("User {Username} registered successfully with email {Email}. UserId: {UserId}", request.Username, request.Email, user.Id);
           
            var activationToken = await _jwtService.GenerateToken(user);

            _logger.LogInformation("JWT token generated successfully for user {Username} with email {Email}. UserId: {UserId}", request.Username, request.Email, user.Id);

            return BaseResponse<AuthTokenDto>.Ok(activationToken);
        }
        catch (ImageUploadException ex)
        {
            _logger.LogError(ex, "Failed to upload avatar image for user {Username} with email {Email}. UserId: {UserId}", request.Username, request.Email, userId);
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.ImageSaveError.ToString());
        }
        catch (JwtCreateException ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for user {Username} with email {Email}. UserId: {UserId}", request.Username, request.Email, userId);
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.JwtGenerateError.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while registering user {Username} with email {Email}. UserId: {UserId}", request.Username, request.Email, userId);
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<AuthTokenDto>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _authRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email {Email}", request.Email);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.InvalidCredentials.ToString());
            }

            if (!request.Password.VerifyPassword(user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user {Username}. UserId: {UserId}", user.Username, user.Id);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.InvalidCredentials.ToString());
            }

            _logger.LogInformation("User {Username} logged in successfully. UserId: {UserId}", user.Username, user.Id);

            var token = await _jwtService.GenerateToken(user);
            token.HasProfile = await _userInfoRepository.GetByUserIdAsync(user.Id) != null;

            return BaseResponse<AuthTokenDto>.Ok(token);
        }
        catch (JwtCreateException ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token on login for email {Email}", request.Email);
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.JwtGenerateError.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email {Email}", request.Email);
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<AuthTokenDto>> RefreshAsync(RefreshRequest request)
    {
        try
        {
            var userId = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);

            if (userId == null)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.InvalidRefreshToken.ToString());
            }

            var user = await _authRepository.GetByIdAsync(userId.Value);

            if (user == null)
            {
                _logger.LogWarning("User not found for refresh token. UserId: {UserId}", userId);
                return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.UserNotFound.ToString());
            }

            _logger.LogInformation("Refreshing token for user {Username}. UserId: {UserId}", user.Username, user.Id);

            var token = await _jwtService.GenerateToken(user);

            return BaseResponse<AuthTokenDto>.Ok(token);
        }
        catch (NotFoundInDbException ex)
        {
            _logger.LogError(ex, "Refresh token not found in database");
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.InvalidRefreshToken.ToString());
        }
        catch (JwtCreateException ex)
        {
            _logger.LogError(ex, "Failed to generate token during refresh");
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.JwtGenerateError.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token refresh");
            return BaseResponse<AuthTokenDto>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }

    public async Task<BaseResponse<bool>> LogoutAsync(string refreshToken)
    {
        try
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existing != null)
                await _refreshTokenRepository.DeleteAsync(existing);

            _logger.LogInformation("User logged out");
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout");
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError.ToString());
        }
    }
}
