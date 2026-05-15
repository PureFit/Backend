using Backend.Core.Entities.TrainingRelated;

namespace Backend.Application.Repositories;

public interface IPlanRepository
{
    Task<AIPlan?> GetByIdWithDetailsAsync(Guid planId);
    Task AddAsync(AIPlan plan);
    Task UpdateAsync(AIPlan plan);
    Task DeleteAsync(AIPlan plan);
}
