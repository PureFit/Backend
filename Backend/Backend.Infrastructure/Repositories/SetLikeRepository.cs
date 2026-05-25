using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class SetLikeRepository : ISetLikeRepository
{
    private readonly AppDbContext _db;

    public SetLikeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SetLike?> GetBySetAndUserAsync(Guid setId, Guid userId) =>
        await _db.SetLikes.FirstOrDefaultAsync(l => l.TrainingSetId == setId && l.UserId == userId);

    public async Task AddAsync(SetLike like)
    {
        await _db.SetLikes.AddAsync(like);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SetLike like)
    {
        _db.SetLikes.Update(like);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(SetLike like)
    {
        _db.SetLikes.Remove(like);
        await _db.SaveChangesAsync();
    }

    public async Task<(int Likes, int Dislikes)> GetCountsAsync(Guid setId)
    {
        var likes    = await _db.SetLikes.CountAsync(l => l.TrainingSetId == setId && l.IsLike);
        var dislikes = await _db.SetLikes.CountAsync(l => l.TrainingSetId == setId && !l.IsLike);
        return (likes, dislikes);
    }

    public async Task<Dictionary<Guid, (int Likes, int Dislikes)>> GetCountsBulkAsync(IEnumerable<Guid> setIds)
    {
        var ids = setIds.ToList();
        var grouped = await _db.SetLikes
            .Where(l => ids.Contains(l.TrainingSetId))
            .GroupBy(l => l.TrainingSetId)
            .Select(g => new
            {
                SetId    = g.Key,
                Likes    = g.Count(l => l.IsLike),
                Dislikes = g.Count(l => !l.IsLike)
            })
            .ToListAsync();

        return grouped.ToDictionary(g => g.SetId, g => (g.Likes, g.Dislikes));
    }

    public async Task<Dictionary<Guid, bool?>> GetUserVotesBulkAsync(Guid userId, IEnumerable<Guid> setIds)
    {
        var ids = setIds.ToList();
        var votes = await _db.SetLikes
            .Where(l => l.UserId == userId && ids.Contains(l.TrainingSetId))
            .Select(l => new { l.TrainingSetId, l.IsLike })
            .ToListAsync();

        return votes.ToDictionary(v => v.TrainingSetId, v => (bool?)v.IsLike);
    }
}
