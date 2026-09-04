namespace Backend.Application.Services;

public interface IAppSettingService
{
    Task<string?> GetAsync(string key);
    Task<T> GetAsync<T>(string key, T defaultValue);
    Task SetAsync(string key, string value);
    Task<Dictionary<string, string>> GetAllAsync();
}
