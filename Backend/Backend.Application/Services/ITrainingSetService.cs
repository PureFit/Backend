using Backend.Application.Common;
using Backend.Application.DTOs.TrainingSet;

namespace Backend.Application.Services;

public interface ITrainingSetService
{
    Task<BaseResponse<TrainingSetResponse>> GetTrainingSetByIdAsync(Guid setId, Guid userId);
    Task<BaseResponse<TrainingSetPagedResult>> GetTrainingSetsByFilterAsync(TrainingSetFilter filter, Guid userId);

    Task<BaseResponse<bool>> CreateTrainingSet(CreateSetRequest request);
    Task<BaseResponse<bool>> UpdateTrainingSet(UpdateSetRequest request, Guid userId);
    Task<BaseResponse<bool>> DeleteTrainingSet(Guid setId, Guid userId);

    Task<BaseResponse<bool>> AddSetBlock(AddSetBlockRequest request);
    Task<BaseResponse<bool>> UpdateSetBlock(UpdateSetBlockRequest request, Guid userId);
    Task<BaseResponse<bool>> DeleteSetBlock(Guid blockId, Guid userId);

    Task<BaseResponse<bool>> AddExerciseEntryToSetBlock(AddExerciseEntryToSetBlockRequest request);
    Task<BaseResponse<bool>> UpdateSetBlockExerciseEntry(UpdateSetBlockExerciseEntryRequest request, Guid userId);
    Task<BaseResponse<bool>> DeleteExerciseEntry(Guid entryId, Guid userId);
    Task<BaseResponse<List<TrainingSetResponse>>> GetPublicSetsByUserAsync(Guid createdByUserId);
}
