namespace Backend.Application.Services;

public record GoogleUserInfo(
    string GoogleId,
    string Email,
    string Name,
    string? PictureUrl
);

public record GoogleTokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Email
);

public interface IGoogleOAuthService
{
    Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken);
    Task<GoogleTokenPair?> ExchangeAuthCodeAsync(string serverAuthCode);
    Task<(string AccessToken, DateTime ExpiresAt)?> RefreshAccessTokenAsync(string refreshToken);
    Task<List<Backend.Application.DTOs.Profile.GoogleCalendarEventDto>> GetCalendarEventsAsync(
        string accessToken, DateTime from, DateTime to);
}
