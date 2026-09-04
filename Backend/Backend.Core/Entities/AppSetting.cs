namespace Backend.Core.Entities;

/// <summary>
/// Key-value конфиг приложения, меняется без передеплоя через AdminController.
/// </summary>
public class AppSetting
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
