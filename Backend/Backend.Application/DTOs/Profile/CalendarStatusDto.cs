namespace Backend.Application.DTOs.Profile;

public class CalendarStatusDto
{
    public bool IsConnected { get; set; }
    public bool IsActive { get; set; }
    public string? ConnectedEmail { get; set; }
}
