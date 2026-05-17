using Backend.Core.Entities.TrainingRelated;

namespace Backend.Application.Repositories;

public interface ITrainingSessionRepository
{
    Task<Guid> CreateAsync(TrainingSession session);
    Task<TrainingSession?> GetByIdAsync(Guid sessionId, Guid userInfoId);
    Task UpdateAsync(TrainingSession session);
    Task<(List<TrainingSession> Items, int TotalCount)> GetHistoryAsync(Guid userInfoId, int page, int pageSize);
}
