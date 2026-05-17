using Backend.Application.Common;

namespace Backend.Application.Services;

public interface IMuscleCalculatorService
{
    Task<BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>> CalculateForSetAsync(Guid setId, float userWeightKg);

    /// <summary>
    /// Возвращает сырые объёмы нагрузки (без нормализации) для инкрементального накопления в UserWorkloadStat.
    /// </summary>
    Task<BaseResponse<(Dictionary<string, float> MuscleVolumes, Dictionary<string, float> BodyPartVolumes)>> CalculateRawVolumeForSetAsync(Guid setId, float userWeightKg);
}
