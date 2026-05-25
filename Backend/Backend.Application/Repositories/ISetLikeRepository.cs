using Backend.Core.Entities.TrainingRelated;

namespace Backend.Application.Repositories;

public interface ISetLikeRepository
{
    Task<SetLike?> GetBySetAndUserAsync(Guid setId, Guid userId);
    Task AddAsync(SetLike like);
    Task UpdateAsync(SetLike like);
    Task DeleteAsync(SetLike like);
    Task<(int Likes, int Dislikes)> GetCountsAsync(Guid setId);
    Task<Dictionary<Guid, (int Likes, int Dislikes)>> GetCountsBulkAsync(IEnumerable<Guid> setIds);
    Task<Dictionary<Guid, bool?>> GetUserVotesBulkAsync(Guid userId, IEnumerable<Guid> setIds);
}
