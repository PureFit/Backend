using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class TrainingSessionRepository : ITrainingSessionRepository
{
    private readonly AppDbContext _db;

    public TrainingSessionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(TrainingSession session)
    {
        await _db.TrainingSessions.AddAsync(session);
        await _db.SaveChangesAsync();
        return session.Id;
    }

    public async Task<TrainingSession?> GetByIdAsync(Guid sessionId, Guid userInfoId)
    {
        return await _db.TrainingSessions
            .Include(s => s.TrainingSet)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserInfoId == userInfoId);
    }

    public async Task UpdateAsync(TrainingSession session)
    {
        _db.TrainingSessions.Update(session);
        await _db.SaveChangesAsync();
    }

    public async Task<(List<TrainingSession> Items, int TotalCount)> GetHistoryAsync(
        Guid userInfoId, int page, int pageSize)
    {
        var query = _db.TrainingSessions
            .Include(s => s.TrainingSet)
            .Where(s => s.UserInfoId == userInfoId)
            .OrderByDescending(s => s.Start);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
