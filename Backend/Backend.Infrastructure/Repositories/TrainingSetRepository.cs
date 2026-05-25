using Backend.Application.DTOs.TrainingSet;
using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class TrainingSetRepository : ITrainingSetRepository
{
    private readonly AppDbContext _db;

    public TrainingSetRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TrainingSet?> GetByIdAsync(Guid setId)
    {
        return await _db.TrainingSets
            .Include(s => s.CreatedBy)
            .Include(s => s.SetBlocks.OrderBy(b => b.Order))
                .ThenInclude(b => b.ExerciseEntries.OrderBy(e => e.Order))
                    .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
            .Include(s => s.SetBlocks)
                .ThenInclude(b => b.ExerciseEntries)
                    .ThenInclude(e => e.ExerciseType)
            .Include(s => s.SetBlocks)
                .ThenInclude(b => b.ExerciseEntries)
                    .ThenInclude(e => e.Exercise)
                        .ThenInclude(ex => ex.ExerciseMuscles)
                            .ThenInclude(em => em.Muscle)
            .Include(s => s.SetBlocks)
                .ThenInclude(b => b.ExerciseEntries)
                    .ThenInclude(e => e.Exercise)
                        .ThenInclude(ex => ex.ExerciseBodyParts)
                            .ThenInclude(eb => eb.BodyPart)
            .FirstOrDefaultAsync(s => s.Id == setId);
    }

    public async Task<(List<TrainingSet> Items, int TotalCount)> GetByFilterAsync(
        TrainingSetFilter filter, Guid userId)
    {
        var query = _db.TrainingSets.AsQueryable();

        // Visibility:
        // OnlyMine=true          → только мои
        // CreatedByUserId=guid   → публичные сеты конкретного юзера (чужие приватные не видны)
        // ничего                 → публичные + мои
        if (filter.OnlyMine)
            query = query.Where(s => s.CreatedByUserId == userId);
        else if (filter.CreatedByUserId.HasValue)
            query = query.Where(s => s.CreatedByUserId == filter.CreatedByUserId.Value &&
                (s.SetAccessType == SetAccessType.Public || s.CreatedByUserId == userId));
        else
            // All tab: base app sets (no owner) + public sets from OTHER users only
            query = query.Where(s => s.CreatedByUserId == null ||
                (s.SetAccessType == SetAccessType.Public && s.CreatedByUserId != userId));

        if (filter.AccessType.HasValue)
            query = query.Where(s => s.SetAccessType == filter.AccessType.Value);

        if (filter.TrainingType.HasValue)
            query = query.Where(s => s.TrainingType == filter.TrainingType.Value);

        if (filter.BodyPartFocus.HasValue)
            query = query.Where(s => s.BodyPartFocus == filter.BodyPartFocus.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            query = query.Where(s => EF.Functions.ILike(s.Name, $"%{filter.SearchQuery}%"));

        if (filter.ExcludeBodyPartFocus != null && filter.ExcludeBodyPartFocus.Count > 0)
            query = query.Where(s => s.BodyPartFocus == null ||
                !filter.ExcludeBodyPartFocus.Contains(s.BodyPartFocus.Value));

        var totalCount = await query.CountAsync();

        var defaultItems = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(s => s.CreatedBy)
            .Include(s => s.SetBlocks.OrderBy(b => b.Order))
                .ThenInclude(b => b.ExerciseEntries.OrderBy(e => e.Order))
                    .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
            .ToListAsync();

        return (defaultItems, totalCount);
    }

    public async Task<Guid> AddAsync(TrainingSet trainingSet)
    {
        await _db.TrainingSets.AddAsync(trainingSet);
        await _db.SaveChangesAsync();
        return trainingSet.Id;
    }

    public async Task UpdateAsync(TrainingSet trainingSet)
    {
        _db.TrainingSets.Update(trainingSet);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TrainingSet trainingSet)
    {
        _db.TrainingSets.Remove(trainingSet);
        await _db.SaveChangesAsync();
    }

    public async Task<List<TrainingSet>> GetPublicByUserAsync(Guid createdByUserId)
    {
        return await _db.TrainingSets
            .Include(s => s.CreatedBy)
            .Include(s => s.SetBlocks.OrderBy(b => b.Order))
                .ThenInclude(b => b.ExerciseEntries.OrderBy(e => e.Order))
                    .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
            .Include(s => s.SetBlocks)
                .ThenInclude(b => b.ExerciseEntries)
                    .ThenInclude(e => e.ExerciseType)
            .Where(s => s.CreatedByUserId == createdByUserId && s.SetAccessType == SetAccessType.Public)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<SetBlock?> GetBlockByIdAsync(Guid blockId)
    {
        return await _db.SetBlocks
            .Include(b => b.ExerciseEntries.OrderBy(e => e.Order))
                .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
            .FirstOrDefaultAsync(b => b.Id == blockId);
    }

    public async Task AddBlockAsync(SetBlock block)
    {
        await _db.SetBlocks.AddAsync(block);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateBlockAsync(SetBlock block)
    {
        _db.SetBlocks.Update(block);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteBlockAsync(SetBlock block)
    {
        _db.SetBlocks.Remove(block);
        await _db.SaveChangesAsync();
    }

    public async Task<ExerciseEntry?> GetEntryByIdAsync(Guid entryId)
    {
        return await _db.ExerciseEntries
            .Include(e => e.Intervals.OrderBy(i => i.Order))
            .FirstOrDefaultAsync(e => e.Id == entryId);
    }

    public async Task AddEntryAsync(ExerciseEntry entry)
    {
        await _db.ExerciseEntries.AddAsync(entry);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateEntryAsync(ExerciseEntry entry)
    {
        _db.ExerciseEntries.Update(entry);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteEntryAsync(ExerciseEntry entry)
    {
        _db.ExerciseEntries.Remove(entry);
        await _db.SaveChangesAsync();
    }

    public async Task<(int TotalSessions, int UniqueUsers)> GetSessionCountsAsync(Guid setId)
    {
        var sessions = _db.TrainingSessions.Where(s =>
            s.TrainingSetId == setId &&
            s.Status == Core.Enums.SessionStatus.Completed);
        var total = await sessions.CountAsync();
        var unique = await sessions.Select(s => s.UserInfoId).Distinct().CountAsync();
        return (total, unique);
    }

    public async Task<Dictionary<Guid, (int TotalSessions, int UniqueUsers)>> GetSessionCountsBulkAsync(IEnumerable<Guid> setIds)
    {
        var ids = setIds.ToList();
        var grouped = await _db.TrainingSessions
            .Where(s => s.TrainingSetId.HasValue && ids.Contains(s.TrainingSetId.Value) && s.Status == Core.Enums.SessionStatus.Completed)
            .GroupBy(s => s.TrainingSetId!.Value)
            .Select(g => new
            {
                SetId = g.Key,
                Total = g.Count(),
                Unique = g.Select(s => s.UserInfoId).Distinct().Count()
            })
            .ToListAsync();

        return grouped.ToDictionary(
            g => g.SetId,
            g => (g.Total, g.Unique)
        );
    }
}
