using Backend.Application.DTOs.Profile;
using Backend.Core.Entities;

namespace Backend.Application.Mappers;

public static class UserInfoMapper
{
    public static UserInfo ToEntity(this CompleteProfileRequest request, Guid userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Sex = request.Sex,
            Level = request.FitnessLevel,
            WeightKg = request.WeightKg,
            HeightCm = request.HeightCm,
            DateOfBirth = request.DateOfBirth
        };

    public static void UpdateEntity(this CompleteProfileRequest request, UserInfo existing)
    {
        existing.Sex = request.Sex;
        existing.Level = request.FitnessLevel;
        existing.WeightKg = request.WeightKg;
        existing.HeightCm = request.HeightCm;
        existing.DateOfBirth = request.DateOfBirth;
    }
}
