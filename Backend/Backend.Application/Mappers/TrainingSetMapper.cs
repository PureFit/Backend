using Backend.Application.DTOs.TrainingSet;
using Backend.Core.Entities.TrainingRelated;

namespace Backend.Application.Mappers;

public static class TrainingSetMapper
{
    public static TrainingSetResponse ToDto(this TrainingSet set) => new()
    {
        Id = set.Id,
        Name = set.Name,
        Description = set.Description,
        ImageUrl = set.ImageUrl,
        CreatedAt = set.CreatedAt,
        SetAccessType = set.SetAccessType.ToString(),
        TrainingType = set.TrainingType?.ToString(),
        BodyPartFocus = set.BodyPartFocus?.ToString(),
        CreatedByUserId = set.CreatedByUserId,
        MusclePercentages = set.MusclePercentages,
        BodyPartPercentages = set.BodyPartPercentages,
        SetBlocks = set.SetBlocks.Select(b => b.ToDto()).ToList()
    };

    public static SetBlockResponse ToDto(this SetBlock block) => new()
    {
        Id = block.Id,
        Order = block.Order,
        SetsCount = block.SetsCount,
        RestBetweenSetsSeconds = block.RestBetweenSetsSeconds,
        RestAfterBlockSeconds = block.RestAfterBlockSeconds,
        ExerciseEntries = block.ExerciseEntries.Select(e => e.ToDto()).ToList()
    };

    public static ExerciseEntryResponse ToDto(this ExerciseEntry entry) => new()
    {
        Id = entry.Id,
        ExerciseId = entry.ExerciseId,
        Order = entry.Order,
        MeasureType = entry.MeasureType.ToString(),
        TargetMuscles = entry.TargetMuscles,
        SecondaryMuscles = entry.SecondaryMuscles,
        BodyParts = entry.BodyParts,
        Reps = entry.Reps,
        DurationSeconds = entry.DurationSeconds,
        Parameters = entry.Parameters,
        RestAfterCurrentEntrySeconds = entry.RestAfterCurrentEntrySeconds,
        Intervals = entry.Intervals.Select(i => new ExerciseIntervalResponse
        {
            Id = i.Id,
            Order = i.Order,
            DurationSeconds = i.DurationSeconds,
            Reps = i.Reps,
            Parameters = i.Parameters
        }).ToList()
    };
}
