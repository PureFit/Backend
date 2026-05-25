namespace Backend.Application.DTOs.Social;

public class UserSearchResultDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string? AvatarUrl { get; set; }
    /// <summary>null = незнакомы, "Pending" = запрос отправлен/получен, "Accepted" = друзья</summary>
    public string? FriendshipStatus { get; set; }
    /// <summary>true если текущий пользователь является инициатором запроса</summary>
    public bool IsRequester { get; set; }
    public Guid? FriendshipId { get; set; }
}
