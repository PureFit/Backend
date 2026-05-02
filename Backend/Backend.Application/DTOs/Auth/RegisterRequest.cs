using Microsoft.AspNetCore.Http;

namespace Backend.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PasswordConfirm { get; set; } = null!;
    public IFormFile? AvatarImg { get; set; }
}
