namespace Backend.Application.Common;

public class GoogleSettings
{
    public string WebClientId { get; set; } = null!;
    public string WebClientSecret { get; set; } = null!;
    public string TokenEndpoint { get; set; } = null!;
    public string TokenInfoEndpoint { get; set; } = null!;
    public string CalendarEventsEndpoint { get; set; } = null!;
    public string AuthCodeRedirectUri { get; set; } = null!;
}
