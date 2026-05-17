using Backend.Application.Repositories;
using Backend.Core.Entities;
using Backend.Core.Entities.TrainingRelated;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _dbContext;

    public PlanRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AIPlan?> GetByIdWithDetailsAsync(Guid planId)
    {
        return await _dbContext.AiPlans
            .Include(p => p.WeekPlans)
                .ThenInclude(w => w.PlanTrainings)
                    .ThenInclude(t => t.TrainingSet)
                        .ThenInclude(ts => ts.SetBlocks)
                            .ThenInclude(b => b.ExerciseEntries)
                                .ThenInclude(e => e.Intervals)
            .Include(p => p.WeekPlans)
                .ThenInclude(w => w.PlanTrainings)
                    .ThenInclude(t => t.TrainingSession)
            .FirstOrDefaultAsync(p => p.Id == planId);
    }

    public async Task AddAsync(AIPlan plan)
    {
        await _dbContext.AiPlans.AddAsync(plan);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(AIPlan plan)
    {
        _dbContext.AiPlans.Update(plan);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(AIPlan plan)
    {
        _dbContext.AiPlans.Remove(plan);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Загружает PlanTraining вместе с TrainingSession.
    /// Проверяет принадлежность пользователю через цепочку WeekPlan → AIPlan → UserInfo.
    /// </summary>
    public async Task<bool> HasCompletedSessionForSetAsync(Guid trainingSetId, Guid userId)
    {
        return await _dbContext.TrainingSessions
            .AnyAsync(x => userId == x.UserInfo.UserId && trainingSetId == x.TrainingSetId);
    }
}
