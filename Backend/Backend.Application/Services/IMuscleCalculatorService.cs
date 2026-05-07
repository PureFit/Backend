using Backend.Application.Common;

namespace Backend.Application.Services;

public interface IMuscleCalculatorService
{
    Task<BaseResponse<(Dictionary<string, float> MusclePercentages, Dictionary<string, float> BodyPartPercentages)>> CalculateForSetAsync(Guid setId);
}
