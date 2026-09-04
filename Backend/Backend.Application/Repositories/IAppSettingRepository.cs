using Backend.Core.Entities;

namespace Backend.Application.Repositories;

public interface IAppSettingRepository
{
    Task<AppSetting?> GetAsync(string key);
    Task<List<AppSetting>> GetAllAsync();
    Task UpsertAsync(string key, string value);
}
