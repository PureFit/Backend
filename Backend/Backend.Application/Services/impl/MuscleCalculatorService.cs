using Backend.Application.Common;
using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Backend.Core.Entities.ExerciseRelated;
using Backend.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace Backend.Application.Services.impl;

public class MuscleCalculatorService : IMuscleCalculatorService
{
    private readonly ITrainingSetRepository _repo;
    private readonly MuscleCalculatorConfig _muscleCalculatorConfig;
    private readonly ExerciseParameterConfig _paramConfig;
    private readonly ILogger<MuscleCalculatorService> _logger;

    public MuscleCalculatorService(
        ITrainingSetRepository repo,
        IOptions<MuscleCalculatorConfig> muscleCalculatorConfig,
        IOptions<ExerciseParameterConfig> paramConfig,
        ILogger<MuscleCalculatorService> logger)
    {
        _repo = repo;
        _muscleCalculatorConfig = muscleCalculatorConfig.Value;
        _paramConfig = paramConfig.Value;
        _logger = logger;
    }

    private static float SecondsToKilometers(int seconds, float speedKmh) =>
        seconds / 3600f * speedKmh;

    private static float SecondsToReps(int seconds, int repsPerMinute) =>
        (float)seconds * repsPerMinute / 60f;

    private static float ApplyDifficulty(float value, float coefficient) =>
        value * coefficient;

    private static float NormalizeToRepScale(float value, float repCoefficient) =>
        value * repCoefficient;

    private static float NormalizeToDistanceScale(float value, float distanceCoefficient) =>
        value * distanceCoefficient;

    private static float GetWeightComponent(Exercise exercise, float? weightKg, float userWeightKg)
    {
        if (!exercise.AllowsWeight || !weightKg.HasValue) return 1f;

        if (exercise.BaseWeightBodyRatio.HasValue)
        {
            var baseWeight = userWeightKg * exercise.BaseWeightBodyRatio.Value;
            return MathF.Log(weightKg.Value / baseWeight + 1f);
        }

        return MathF.Log(weightKg.Value + 1f);
    }

    private static float GetSpeedModifier(ExerciseType type, float actualSpeedKmh) =>
        type.ReferenceSpeedKmh.HasValue ? actualSpeedKmh / type.ReferenceSpeedKmh.Value : 1f;


    public async Task<BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>> CalculateForSetAsync(Guid setId, float userWeightKg)
    {
        var set = await _repo.GetByIdAsync(setId);

        if (set is null)
        {
            _logger.LogWarning("Muscle calculation failed: Training set with ID {SetId} not found.", setId);
            return BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>.Fail(ErrorEnums.NotFound);
        }

        var musclePercentages = new Dictionary<string, float>();
        var bodyPartPercentages = new Dictionary<string, float>();

        var exercises = set.SetBlocks.SelectMany(sb => sb.ExerciseEntries);

        foreach (var entry in exercises)
        {
            var volume = CalculateVolume(entry, userWeightKg);

            foreach (var em in entry.Exercise.ExerciseMuscles)
            {
                var effectiveVolume = em.Role == MuscleRole.Primary
                    ? volume
                    : volume * _muscleCalculatorConfig.SecondaryCoefficient;

                musclePercentages[em.Muscle.Name] = musclePercentages.GetValueOrDefault(em.Muscle.Name) + effectiveVolume;
            }

            foreach (var eb in entry.Exercise.ExerciseBodyParts)
                bodyPartPercentages[eb.BodyPart.Name] = bodyPartPercentages.GetValueOrDefault(eb.BodyPart.Name) + volume;
        }

        var muscleTotal    = musclePercentages.Values.Sum();
        var bodyPartTotal  = bodyPartPercentages.Values.Sum();

        var muscleResult   = musclePercentages.ToDictionary(x => x.Key, x => muscleTotal > 0 ? x.Value / muscleTotal : 0f);
        var bodyPartResult = bodyPartPercentages.ToDictionary(x => x.Key, x => bodyPartTotal > 0 ? x.Value / bodyPartTotal : 0f);

        return BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>.Ok((muscleResult, bodyPartResult));
    }

    public async Task<BaseResponse<(Dictionary<string, float> MuscleVolumes, Dictionary<string, float> BodyPartVolumes)>> CalculateRawVolumeForSetAsync(Guid setId, float userWeightKg)
    {
        var set = await _repo.GetByIdAsync(setId);
        if (set is null)
            return BaseResponse<(Dictionary<string, float>, Dictionary<string, float>)>.Fail(ErrorEnums.NotFound);

        var muscleVolumes   = new Dictionary<string, float>();
        var bodyPartVolumes = new Dictionary<string, float>();

        foreach (var entry in set.SetBlocks.SelectMany(sb => sb.ExerciseEntries))
        {
            var volume = CalculateVolume(entry, userWeightKg);

            foreach (var em in entry.Exercise.ExerciseMuscles)
            {
                var effective = em.Role == MuscleRole.Primary ? volume : volume * _muscleCalculatorConfig.SecondaryCoefficient;
                muscleVolumes[em.Muscle.Name] = muscleVolumes.GetValueOrDefault(em.Muscle.Name) + effective;
            }

            foreach (var eb in entry.Exercise.ExerciseBodyParts)
                bodyPartVolumes[eb.BodyPart.Name] = bodyPartVolumes.GetValueOrDefault(eb.BodyPart.Name) + volume;
        }

        return BaseResponse<(Dictionary<string, float>, Dictionary<string, float>)>.Ok((muscleVolumes, bodyPartVolumes));
    }

    private float CalculateVolume(ExerciseEntry entry, float userWeightKg)
    {
        float baseVolume = entry.Intervals.Count > 0
            ? entry.Intervals.Sum(i => CalculateSingleVolume(entry.ExerciseType, entry.Exercise, i.Reps, i.DurationSeconds, i.DistanceMeters, i.WeightKg, i.SpeedKmh, userWeightKg))
            : CalculateSingleVolume(entry.ExerciseType, entry.Exercise, entry.Reps, entry.DurationSeconds, entry.DistanceMeters, entry.WeightKg, entry.SpeedKmh, userWeightKg);

        return baseVolume * entry.SetBlock.SetsCount;
    }

    private float CalculateSingleVolume(ExerciseType type, Exercise exercise, int? reps, int? durationSeconds, float? distanceMeters, float? weightKg, float? speedKmh, float userWeightKg)
    {
        return type.MeasureCategory switch
        {
            MeasureCategory.RepsBased  => GetVolumeReps(type, exercise, reps, weightKg, userWeightKg),
            MeasureCategory.DistanceBased => GetVolumeDistance(type, distanceMeters, durationSeconds, speedKmh),
            MeasureCategory.DurationOnly  => GetVolumeDuration(type, exercise, durationSeconds, weightKg, userWeightKg),
            _ => 0f
        };
    }

    private float GetVolumeDuration(ExerciseType type, Exercise exercise, int? durationSeconds, float? weightKg, float userWeightKg)
    {
        if (!durationSeconds.HasValue)
            return 0f;

        if (type.ReferenceSpeedKmh.HasValue)
        {
            var distance         = SecondsToKilometers(durationSeconds.Value, type.ReferenceSpeedKmh.Value);
            var effectiveDistance = ApplyDifficulty(distance, type.Coefficient);
            var normalizedVolume  = NormalizeToDistanceScale(effectiveDistance, _paramConfig.DistanceCoefficient);
            return normalizedVolume;
        }

        if (type.AverageRepsPerMinute.HasValue)
        {
            var estimatedReps    = SecondsToReps(durationSeconds.Value, type.AverageRepsPerMinute.Value);
            var weightComponent  = GetWeightComponent(exercise, weightKg, userWeightKg);
            var effectiveReps    = ApplyDifficulty(estimatedReps * weightComponent, type.Coefficient);
            var normalizedVolume = NormalizeToRepScale(effectiveReps, _paramConfig.RepCoefficient);
            return normalizedVolume;
        }

        return 0f;
    }

    private float GetVolumeDistance(ExerciseType type, float? distanceMeters, int? durationSeconds, float? speedKmh)
    {
        var knownParams = new[] { distanceMeters.HasValue, durationSeconds.HasValue, speedKmh.HasValue };
        if (knownParams.Count(x => x) < 2)
            return 0f;

        float distance = distanceMeters ?? SecondsToKilometers(durationSeconds.Value, speedKmh.Value);
        var speed = speedKmh ?? distanceMeters!.Value / 1000f / (durationSeconds!.Value / 3600f);

        var speedModifier     = GetSpeedModifier(type, speed);
        var effectiveDistance = ApplyDifficulty(distance * speedModifier, type.Coefficient);
        return NormalizeToDistanceScale(effectiveDistance, _paramConfig.DistanceCoefficient);
    }

    private float GetVolumeReps(ExerciseType type, Exercise exercise, int? reps, float? weightKg, float userWeightKg)
    {
        if (!reps.HasValue) return 0f;

        var weightComponent  = GetWeightComponent(exercise, weightKg, userWeightKg);
        var effectiveReps    = ApplyDifficulty(reps.Value * weightComponent, type.Coefficient);
        var normalizedVolume = NormalizeToRepScale(effectiveReps, _paramConfig.RepCoefficient);
        return normalizedVolume;
    }
}
