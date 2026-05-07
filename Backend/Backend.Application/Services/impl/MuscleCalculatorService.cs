using Backend.Application.Common;
using Backend.Application.Repositories;
using Backend.Core.Entities.TrainingRelated;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Application.Services.impl;

public class MuscleCalculatorService : IMuscleCalculatorService
{
    private readonly ITrainingSetRepository _repo;
    private readonly MuscleCalculatorConfig _muscleCalculatorConfig;
    private readonly ExerciseParameterConfig _paramConfig;
    private readonly IReadOnlyDictionary<string, ParameterEntry> _paramLookup;
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
        _paramLookup = _paramConfig.Parameters
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
        _logger = logger;
    }

    public async Task<BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>> CalculateForSetAsync(Guid setId)
    {
        var set = await _repo.GetByIdAsync(setId);

        if (set is null)
        {
            _logger.LogWarning("TrainingSet with Id {SetId} not found for muscle calculation", setId);
            return BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>.Fail("");
        }

        _logger.LogInformation("Calculating muscle and body part percentages for TrainingSet with Id {SetId}", setId);

        var accumResultsMusclesValues = new Dictionary<string, float>();
        var accumResultsBodyPartsValues = new Dictionary<string, float>();

        var entries = set.SetBlocks.SelectMany(sb => sb.ExerciseEntries);

        foreach (var entry in entries)
        {
            var volume = CalculateVolume(entry);

            foreach (var muscle in entry.TargetMuscles)
            {
                if (!accumResultsMusclesValues.ContainsKey(muscle))
                    accumResultsMusclesValues[muscle] = 0;
                accumResultsMusclesValues[muscle] += volume;
            }

            foreach (var muscle in entry.SecondaryMuscles)
            {
                if (!accumResultsMusclesValues.ContainsKey(muscle))
                    accumResultsMusclesValues[muscle] = 0;
                accumResultsMusclesValues[muscle] += volume * _muscleCalculatorConfig.SecondaryCoefficient;
            }
        }

        var musclePercentages = CalculatePercentagesFromValues(accumResultsMusclesValues);
        var bodyPartPercentages = CalculatePercentagesFromValues(accumResultsBodyPartsValues);

        return BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>.Ok((musclePercentages, bodyPartPercentages));
    }

    private Dictionary<string, float> CalculatePercentagesFromValues(Dictionary<string, float> accumResultsValues)
    {
        var totalVolume = accumResultsValues.Values.Sum();
        var muscleCount = accumResultsValues.Count(x => x.Value > 0);

        return accumResultsValues.ToDictionary(x => x.Key, x => muscleCount > 0 ? x.Value / totalVolume : 0);
    }

    private float CalculateVolume(ExerciseEntry entry)
    {
        if (entry.Intervals.Count > 0)
        {
            return entry.Intervals.Sum(GetVolumeInterval);
        }

        if (entry.Reps.HasValue && entry.DurationSeconds.HasValue)
        {
            _logger.LogWarning("ExerciseEntry with Id {EntryId} has both Reps and Duration specified without intervals. This is not supported for volume calculation.", entry.Id);
        }

        float baseVolume;

        if (entry.Reps.HasValue)
        {
            baseVolume = GetVolumeReps(entry);
        }
        else if (entry.DurationSeconds.HasValue)
        {
            baseVolume = GetVolumeDuration(entry);
        }
        else if (entry.DistanceMeters.HasValue)
        {
            baseVolume = entry.DistanceMeters.Value;
        }
        else
        {
            _logger.LogWarning("ExerciseEntry with Id {EntryId} has no measurable data for volume calculation", entry.Id);
            return 0f;
        }

        return ConsiderSets(entry, baseVolume);
    }

    private float GetVolumeReps(ExerciseEntry entry)
    {
        float repMultiplier = 1f;
        float additiveModifier = 1f;

        if (entry.Parameters != null)
        {
            foreach (var (key, value) in entry.Parameters)
            {
                if (!_paramLookup.TryGetValue(key, out var meta))
                    continue;

                switch (meta.Role)
                {
                    case ParameterRole.RepMultiplier:
                        repMultiplier = value;
                        break;
                    case ParameterRole.AdditiveModifier:
                        additiveModifier *= (1f + value * meta.Factor);
                        break;
                }
            }
        }

        return entry.Reps!.Value * repMultiplier * additiveModifier;
    }

    private float GetVolumeDuration(ExerciseEntry entry)
    {
        float durationLoad = 0f;
        float additiveModifier = 1f;
        bool hasDurationIntensity = false;

        if (entry.Parameters != null)
        {
            foreach (var (key, value) in entry.Parameters)
            {
                if (!_paramLookup.TryGetValue(key, out var meta))
                    continue;

                switch (meta.Role)
                {
                    case ParameterRole.DurationIntensity:
                        durationLoad += value * meta.Factor;
                        hasDurationIntensity = true;
                        break;
                    case ParameterRole.AdditiveModifier:
                        additiveModifier *= (1f + value * meta.Factor);
                        break;
                }
            }
        }

        float baseVolume = hasDurationIntensity
            ? entry.DurationSeconds!.Value * durationLoad
            : entry.DurationSeconds!.Value / _paramConfig.SecondsPerRepEquivalent;

        return baseVolume * additiveModifier;
    }

    private float GetVolumeInterval(ExerciseInterval interval)
    {
        float repMultiplier = 1f;
        float speedKmh = 0f;
        float additiveModifier = 1f;
        bool hasDurationIntensity = false;

        if (interval.Parameters != null)
        {
            foreach (var (key, value) in interval.Parameters)
            {
                if (!_paramLookup.TryGetValue(key, out var meta))
                    continue;

                switch (meta.Role)
                {
                    case ParameterRole.RepMultiplier:
                        repMultiplier = value;
                        break;
                    case ParameterRole.SpeedKmh:
                        speedKmh += value;
                        hasDurationIntensity = true;
                        break;
                    case ParameterRole.AdditiveModifier:
                        additiveModifier *= (1f + value * meta.Factor);
                        break;
                }
            }
        }

        float baseVolume;

        if (interval.Reps.HasValue)
        {
            baseVolume = interval.Reps.Value * repMultiplier;
        }
        else if (hasDurationIntensity)
        {
            baseVolume = interval.DurationSeconds * durationLoad;
        }
        else
        {
            baseVolume = interval.DurationSeconds / _paramConfig.SecondsPerRepEquivalent;
        }

        return baseVolume * additiveModifier;
    }

    private float ConsiderSets(ExerciseEntry entry, float baseVolume)
    {
        return baseVolume * entry.SetBlock.SetsCount;
    }
}
