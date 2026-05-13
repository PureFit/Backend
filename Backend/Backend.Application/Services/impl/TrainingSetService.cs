using Backend.Application.Common;
using Backend.Application.DTOs.TrainingSet;
using Backend.Application.Mappers;
using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class TrainingSetService : ITrainingSetService
{
    private readonly ITrainingSetRepository _repo;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IMuscleCalculatorService _muscleCalculator;
    private readonly ILogger<TrainingSetService> _logger;

    public TrainingSetService(
        ITrainingSetRepository repo,
        IUserInfoRepository userInfoRepository,
        IMuscleCalculatorService muscleCalculator,
        ILogger<TrainingSetService> logger)
    {
        _repo = repo;
        _userInfoRepository = userInfoRepository;
        _muscleCalculator = muscleCalculator;
        _logger = logger;
    }

    private async Task RecalculateMusclesAsync(TrainingSet set, Guid userId)
    {
        var userInfo = await _userInfoRepository.GetByUserIdAsync(userId);
        if (userInfo is null) return;

        var result = await _muscleCalculator.CalculateForSetAsync(set.Id, (float)userInfo.WeightKg);
        if (!result.Success) return;

        set.MusclePercentages = result.Data.MusclePercentages;
        set.BodyPartPercentages = result.Data.BodyPartPercentages;
        await _repo.UpdateAsync(set);
    }

    public async Task<BaseResponse<TrainingSetResponse>> GetTrainingSetByIdAsync(Guid setId, Guid userId)
    {
        try
        {
            var setDb = await _repo.GetByIdAsync(setId);

            if (setDb is null)
            {
                _logger.LogWarning("Training set {SetId} not found", setId);
                return BaseResponse<TrainingSetResponse>.Fail(ErrorEnums.NotFound);
            }

            if (setDb.SetAccessType == SetAccessType.Private && setDb.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access private training set {SetId}", userId, setId);
                return BaseResponse<TrainingSetResponse>.Fail(ErrorEnums.Forbidden);
            }

            return BaseResponse<TrainingSetResponse>.Ok(setDb.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrainingSetByIdAsync failed for SetId={SetId}", setId);
            return BaseResponse<TrainingSetResponse>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<TrainingSetPagedResult>> GetTrainingSetsByFilterAsync(TrainingSetFilter filter, Guid userId)
    {
        try
        {
            var matchingSets = await _repo.GetByFilterAsync(filter, userId);

            return BaseResponse<TrainingSetPagedResult>.Ok(new TrainingSetPagedResult
            {
                Items = matchingSets.Items.Select(s => s.ToDto()).ToList(),
                TotalCount = matchingSets.TotalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrainingSetsByFilterAsync failed");
            return BaseResponse<TrainingSetPagedResult>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> CreateTrainingSet(CreateSetRequest request)
    {
        try
        {
            var set = new TrainingSet
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                SetAccessType = request.SetAccessType,
                TrainingType = request.TrainingType,
                BodyPartFocus = request.BodyPartFocus,
                CreatedByUserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(set);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateTrainingSet failed for UserId={UserId}", request.UserId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> UpdateTrainingSet(UpdateSetRequest request, Guid userId)
    {
        try
        {
            var set = await _repo.GetByIdAsync(request.SetId);
            if (set is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);
            if (set.CreatedByUserId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            if (request.Name is not null) set.Name = request.Name;
            if (request.Description is not null) set.Description = request.Description;
            if (request.ImageUrl is not null) set.ImageUrl = request.ImageUrl;
            if (request.SetAccessType.HasValue) set.SetAccessType = request.SetAccessType.Value;
            if (request.TrainingType.HasValue) set.TrainingType = request.TrainingType;
            if (request.BodyPartFocus.HasValue) set.BodyPartFocus = request.BodyPartFocus;

            await _repo.UpdateAsync(set);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateTrainingSet failed for SetId={SetId}", request.SetId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> DeleteTrainingSet(Guid setId, Guid userId)
    {
        try
        {
            var set = await _repo.GetByIdAsync(setId);
            if (set is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);
            if (set.CreatedByUserId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            await _repo.DeleteAsync(set);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteTrainingSet failed for SetId={SetId}", setId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> AddSetBlock(AddSetBlockRequest request)
    {
        try
        {
            var set = await _repo.GetByIdAsync(request.SetId);
            if (set is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);
            if (set.CreatedByUserId != request.UserId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            var nextOrder = set.SetBlocks.Count > 0 ? set.SetBlocks.Max(b => b.Order) + 1 : 1;

            await _repo.AddBlockAsync(new SetBlock
            {
                Id = Guid.NewGuid(),
                TrainingSetId = request.SetId,
                SetsCount = request.SetsCount,
                RestBetweenSetsSeconds = request.RestBetweenSetsSeconds,
                RestAfterBlockSeconds = request.RestAfterBlockSeconds,
                Order = nextOrder
            });

            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddSetBlock failed for SetId={SetId}", request.SetId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> UpdateSetBlock(UpdateSetBlockRequest request, Guid userId)
    {
        try
        {
            var block = await _repo.GetBlockByIdAsync(request.BlockId);
            if (block is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            var set = await _repo.GetByIdAsync(block.TrainingSetId);
            if (set is null || set.CreatedByUserId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            var setsCountChanged = request.SetsCount.HasValue && request.SetsCount.Value != block.SetsCount;

            if (request.SetsCount.HasValue) block.SetsCount = request.SetsCount.Value;
            if (request.RestBetweenSetsSeconds.HasValue) block.RestBetweenSetsSeconds = request.RestBetweenSetsSeconds.Value;
            if (request.RestAfterBlockSeconds.HasValue) block.RestAfterBlockSeconds = request.RestAfterBlockSeconds.Value;
            if (request.Order.HasValue) block.Order = request.Order.Value;

            await _repo.UpdateBlockAsync(block);

            if (setsCountChanged)
                await RecalculateMusclesAsync(set, userId);

            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateSetBlock failed for BlockId={BlockId}", request.BlockId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> DeleteSetBlock(Guid blockId, Guid userId)
    {
        try
        {
            var block = await _repo.GetBlockByIdAsync(blockId);
            if (block is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            var set = await _repo.GetByIdAsync(block.TrainingSetId);
            if (set is null || set.CreatedByUserId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            await _repo.DeleteBlockAsync(block);
            await RecalculateMusclesAsync(set, userId);

            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteSetBlock failed for BlockId={BlockId}", blockId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> AddExerciseEntryToSetBlock(AddExerciseEntryToSetBlockRequest request)
    {
        try
        {
            var block = await _repo.GetBlockByIdAsync(request.BlockId);
            if (block is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            var set = await _repo.GetByIdAsync(block.TrainingSetId);
            if (set is null || set.CreatedByUserId != request.UserId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            var nextOrder = block.ExerciseEntries.Count > 0 ? block.ExerciseEntries.Max(e => e.Order) + 1 : 1;

            await _repo.AddEntryAsync(new ExerciseEntry
            {
                Id = Guid.NewGuid(),
                SetBlockId = request.BlockId,
                ExerciseId = request.ExerciseId,
                ExerciseTypeId = request.ExerciseTypeId,
                MeasureType = request.MeasureType,
                Reps = request.Reps,
                DurationSeconds = request.DurationSeconds,
                DistanceMeters = request.DistanceMeters,
                WeightKg = request.WeightKg,
                SpeedKmh = request.SpeedKmh,
                RestAfterCurrentEntrySeconds = request.RestAfterCurrentEntrySeconds,
                Order = nextOrder
            });

            await RecalculateMusclesAsync(set, request.UserId);
            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddExerciseEntryToSetBlock failed for BlockId={BlockId}", request.BlockId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> UpdateSetBlockExerciseEntry(UpdateSetBlockExerciseEntryRequest request, Guid userId)
    {
        try
        {
            var entryDb = await _repo.GetEntryByIdAsync(request.EntryId);
            if (entryDb is null)
            {
                _logger.LogWarning("Exercise entry {EntryId} not found", request.EntryId);
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);
            }

            var block = await _repo.GetBlockByIdAsync(entryDb.SetBlockId);
            var set = block is not null ? await _repo.GetByIdAsync(block.TrainingSetId) : null;
            if (set is null || set.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} has no permission to modify entry {EntryId}", userId, request.EntryId);
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);
            }

            var volumeChanged = (request.Reps.HasValue && request.Reps != entryDb.Reps) ||
                                (request.DurationSeconds.HasValue && request.DurationSeconds != entryDb.DurationSeconds) ||
                                (request.DistanceMeters.HasValue && request.DistanceMeters != entryDb.DistanceMeters) ||
                                (request.WeightKg.HasValue && request.WeightKg != entryDb.WeightKg) ||
                                (request.SpeedKmh.HasValue && request.SpeedKmh != entryDb.SpeedKmh);

            if (request.Reps.HasValue) entryDb.Reps = request.Reps;
            if (request.DurationSeconds.HasValue) entryDb.DurationSeconds = request.DurationSeconds;
            if (request.DistanceMeters.HasValue) entryDb.DistanceMeters = request.DistanceMeters;
            if (request.WeightKg.HasValue) entryDb.WeightKg = request.WeightKg;
            if (request.SpeedKmh.HasValue) entryDb.SpeedKmh = request.SpeedKmh;
            if (request.RestAfterCurrentEntrySeconds.HasValue) entryDb.RestAfterCurrentEntrySeconds = request.RestAfterCurrentEntrySeconds.Value;
            if (request.Order.HasValue) entryDb.Order = request.Order.Value;
            if (request.MeasureType.HasValue) entryDb.MeasureType = request.MeasureType.Value;

            await _repo.UpdateEntryAsync(entryDb);

            if (volumeChanged)
                await RecalculateMusclesAsync(set, userId);

            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateSetBlockExerciseEntry failed for EntryId={EntryId}", request.EntryId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }

    public async Task<BaseResponse<bool>> DeleteExerciseEntry(Guid entryId, Guid userId)
    {
        try
        {
            var entry = await _repo.GetEntryByIdAsync(entryId);
            if (entry is null)
                return BaseResponse<bool>.Fail(ErrorEnums.NotFound);

            var block = await _repo.GetBlockByIdAsync(entry.SetBlockId);
            var set = block is not null ? await _repo.GetByIdAsync(block.TrainingSetId) : null;
            if (set is null || set.CreatedByUserId != userId)
                return BaseResponse<bool>.Fail(ErrorEnums.Forbidden);

            await _repo.DeleteEntryAsync(entry);
            await RecalculateMusclesAsync(set, userId);

            return BaseResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteExerciseEntry failed for EntryId={EntryId}", entryId);
            return BaseResponse<bool>.Fail(ErrorEnums.UnknownError);
        }
    }
}
