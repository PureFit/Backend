using Backend.Core.Entities.TrainingRelated;

namespace Backend.Core.Entities;

public enum AuthProvider
{
    Local,
    Google
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public string? AvatarUrl { get; set; }
    public string? GoogleId { get; set; }
    public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserInfo UserInfo { get; set; } = null!;
    public UserGoogleToken? GoogleToken { get; set; }
}
