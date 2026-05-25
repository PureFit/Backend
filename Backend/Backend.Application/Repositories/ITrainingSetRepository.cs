using Backend.Application.DTOs.TrainingSet;
using Backend.Core.Entities.TrainingRelated;

namespace Backend.Application.Repositories;

public interface ITrainingSetRepository
{
    // TrainingSet
    Task<TrainingSet?> GetByIdAsync(Guid setId);
    Task<(int TotalSessions, int UniqueUsers)> GetSessionCountsAsync(Guid setId);
    Task<Dictionary<Guid, (int TotalSessions, int UniqueUsers)>> GetSessionCountsBulkAsync(IEnumerable<Guid> setIds);
    Task<(List<TrainingSet> Items, int TotalCount)> GetByFilterAsync(TrainingSetFilter filter, Guid userId);
    Task<Guid> AddAsync(TrainingSet trainingSet);
    Task UpdateAsync(TrainingSet trainingSet);
    Task DeleteAsync(TrainingSet trainingSet);
    Task<List<TrainingSet>> GetPublicByUserAsync(Guid createdByUserId);

    // SetBlock
    Task<SetBlock?> GetBlockByIdAsync(Guid blockId);
    Task AddBlockAsync(SetBlock block);
    Task UpdateBlockAsync(SetBlock block);
    Task DeleteBlockAsync(SetBlock block);

    // ExerciseEntry
    Task<ExerciseEntry?> GetEntryByIdAsync(Guid entryId);
    Task AddEntryAsync(ExerciseEntry entry);
    Task UpdateEntryAsync(ExerciseEntry entry);
    Task DeleteEntryAsync(ExerciseEntry entry);
}
