using Backend.Application.Common;
using Backend.Application.DTOs.Session;

namespace Backend.Application.Services;

public interface ITrainingSessionService
{
    Task<BaseResponse<TrainingSessionDto>> StartSessionAsync(Guid userId, StartSessionRequest request);
    Task<BaseResponse<TrainingSessionDto>> FinishSessionAsync(Guid userId, Guid sessionId);
    Task<BaseResponse<TrainingSessionDto>> AbandonSessionAsync(Guid userId, Guid sessionId);
    Task<BaseResponse<TrainingSessionDto>> GetByIdAsync(Guid userId, Guid sessionId);
    Task<BaseResponse<SessionHistoryResponse>> GetHistoryAsync(Guid userId, int page, int pageSize);
    Task<BaseResponse<TrainingSessionDto>> SaveProgressAsync(Guid userId, Guid sessionId, int stepIndex);
    Task<BaseResponse<TrainingSessionDto?>> GetActiveBySetAsync(Guid userId, Guid trainingSetId);
}
