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
        SetBlocks = set.SetBlocks.Select(b => b.ToDto()).ToList()
    };

    public static SetBlockResponse ToDto(this SetBlock block) => new()
    {
        Id = block.Id,
        Name = block.Name,
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
        ExerciseName = entry.Exercise?.Name,
        ExerciseTypeId = entry.ExerciseTypeId,
        Order = entry.Order,
        Reps = entry.Reps,
        DurationSeconds = entry.DurationSeconds,
        DistanceMeters = entry.DistanceMeters,
        WeightKg = entry.WeightKg,
        SpeedKmh = entry.SpeedKmh,
        RestAfterCurrentEntrySeconds = entry.RestAfterCurrentEntrySeconds,
        Intervals = entry.Intervals.Select(i => new ExerciseIntervalResponse
        {
            Id = i.Id,
            Order = i.Order,
            Reps = i.Reps,
            DurationSeconds = i.DurationSeconds,
            DistanceMeters = i.DistanceMeters,
            WeightKg = i.WeightKg,
            SpeedKmh = i.SpeedKmh
        }).ToList()
    };
}
