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
            .Include(s => s.SetBlocks.OrderBy(b => b.Order))
                .ThenInclude(b => b.ExerciseEntries.OrderBy(e => e.Order))
                    .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
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
            query = query.Where(s => s.SetAccessType == SetAccessType.Public || s.CreatedByUserId == userId);

        if (filter.AccessType.HasValue)
            query = query.Where(s => s.SetAccessType == filter.AccessType.Value);

        if (filter.TrainingType.HasValue)
            query = query.Where(s => s.TrainingType == filter.TrainingType.Value);

        if (filter.BodyPartFocus.HasValue)
            query = query.Where(s => s.BodyPartFocus == filter.BodyPartFocus.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            query = query.Where(s => s.Name.Contains(filter.SearchQuery));

        if (filter.ExcludeMuscleNames != null && filter.ExcludeMuscleNames.Count > 0)
            query = query.Where(s => s.MusclePercentages == null ||
                !filter.ExcludeMuscleNames.Any(m => s.MusclePercentages.ContainsKey(m)));

        if (filter.ExcludeBodyPartFocus != null && filter.ExcludeBodyPartFocus.Count > 0)
            query = query.Where(s => s.BodyPartFocus == null ||
                !filter.ExcludeBodyPartFocus.Contains(s.BodyPartFocus.Value));

        var totalCount = await query.CountAsync();

        // подгружаем без пагинации только если нужна сортировка по мышцам
        if (filter.SortByMuscles != null && filter.SortByMuscles.Count > 0)
        {
            var all = await query
                .Include(s => s.SetBlocks.OrderBy(b => b.Order))
                    .ThenInclude(b => b.ExerciseEntries.OrderBy(e => e.Order))
                        .ThenInclude(e => e.Intervals.OrderBy(i => i.Order))
                .ToListAsync();

            var items = all
                .OrderByDescending(s => filter.SortByMuscles
                    .Sum(m => s.MusclePercentages?.GetValueOrDefault(m) ?? 0))
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return (items, totalCount);
        }

        var defaultItems = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
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
}
