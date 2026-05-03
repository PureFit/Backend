using System.Text.Json;
using Backend.Application.Common;
using Backend.Application.Services;
using Backend.Application.DTOs.Profile;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Services;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly GoogleSettings _settings;
    private readonly ILogger<GoogleOAuthService> _logger;
    private readonly HttpClient _httpClient;

    public GoogleOAuthService(
        IOptions<GoogleSettings> settings,
        ILogger<GoogleOAuthService> logger,
        HttpClient httpClient)
    {
        _settings = settings.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _settings.WebClientId }
                });

            return new GoogleUserInfo(
                GoogleId: payload.Subject,
                Email: payload.Email,
                Name: payload.Name ?? payload.Email.Split('@')[0],
                PictureUrl: payload.Picture
            );
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google idToken");
            return null;
        }
    }

    public async Task<GoogleTokenPair?> ExchangeAuthCodeAsync(string serverAuthCode)
    {
        try
        {
            var formFields = new Dictionary<string, string>
            {
                ["code"] = serverAuthCode,
                ["client_id"] = _settings.WebClientId,
                ["client_secret"] = _settings.WebClientSecret,
                ["grant_type"] = "authorization_code"
            };
            if (!string.IsNullOrEmpty(_settings.AuthCodeRedirectUri))
                formFields["redirect_uri"] = _settings.AuthCodeRedirectUri;

            var response = await _httpClient.PostAsync(
                _settings.TokenEndpoint,
                new FormUrlEncodedContent(formFields));

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Google token exchange failed: {Error}", err);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var accessToken = root.GetProperty("access_token").GetString()!;
            var refreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString()! : "";
            var expiresIn = root.GetProperty("expires_in").GetInt32();
            var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

            var infoResp = await _httpClient.GetAsync(
                $"{_settings.TokenInfoEndpoint}?access_token={accessToken}");
            var infoJson = await infoResp.Content.ReadAsStringAsync();
            using var infoDoc = JsonDocument.Parse(infoJson);
            var email = infoDoc.RootElement.TryGetProperty("email", out var emailProp)
                ? emailProp.GetString()!
                : "unknown";

            return new GoogleTokenPair(accessToken, refreshToken, expiresAt, email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to exchange Google auth code");
            return null;
        }
    }

    public async Task<(string AccessToken, DateTime ExpiresAt)?> RefreshAccessTokenAsync(string refreshToken)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                _settings.TokenEndpoint,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["refresh_token"] = refreshToken,
                    ["client_id"] = _settings.WebClientId,
                    ["client_secret"] = _settings.WebClientSecret,
                    ["grant_type"] = "refresh_token"
                }));

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var accessToken = root.GetProperty("access_token").GetString()!;
            var expiresIn = root.GetProperty("expires_in").GetInt32();
            return (accessToken, DateTime.UtcNow.AddSeconds(expiresIn));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh Google access token");
            return null;
        }
    }

    public async Task<List<GoogleCalendarEventDto>> GetCalendarEventsAsync(
        string accessToken, DateTime from, DateTime to)
    {
        try
        {
            var timeMin = Uri.EscapeDataString(from.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            var timeMax = Uri.EscapeDataString(to.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            var url = $"{_settings.CalendarEventsEndpoint}" +
                      $"?timeMin={timeMin}&timeMax={timeMax}&singleEvents=true&orderBy=startTime&maxResults=100";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var events = new List<GoogleCalendarEventDto>();
            if (!doc.RootElement.TryGetProperty("items", out var items)) return events;

            foreach (var item in items.EnumerateArray())
            {
                var id = item.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
                var title = item.TryGetProperty("summary", out var sumProp)
                    ? sumProp.GetString() ?? "(no title)"
                    : "(no title)";

                var startProp = item.GetProperty("start");
                var endProp = item.GetProperty("end");

                bool isAllDay = startProp.TryGetProperty("date", out var startDate);

                DateTime startTime, endTime;
                if (isAllDay)
                {
                    startTime = DateTime.Parse(startDate.GetString()!).ToUniversalTime();
                    endTime = DateTime.Parse(endProp.GetProperty("date").GetString()!).ToUniversalTime();
                }
                else
                {
                    startTime = DateTime.Parse(startProp.GetProperty("dateTime").GetString()!).ToUniversalTime();
                    endTime = DateTime.Parse(endProp.GetProperty("dateTime").GetString()!).ToUniversalTime();
                }

                events.Add(new GoogleCalendarEventDto
                {
                    Id = id,
                    Title = title,
                    StartTime = startTime,
                    EndTime = endTime,
                    DurationMinutes = (int)(endTime - startTime).TotalMinutes,
                    IsAllDay = isAllDay
                });
            }

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Google Calendar events");
            return [];
        }
    }
}
