namespace Backend.Application.DTOs.Profile;

public class GoogleCalendarEventDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsAllDay { get; set; }
}
