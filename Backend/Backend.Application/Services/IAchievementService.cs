using Backend.Application.Common;
using Backend.Application.DTOs.Achievement;
using Backend.Core.Enums;

namespace Backend.Application.Services;

public interface IAchievementService
{
    /// <summary>Все ачивки с флагом isUnlocked для конкретного пользователя</summary>
    Task<BaseResponse<List<AchievementDto>>> GetUserAchievementsAsync(Guid userId);

    /// <summary>
    /// Проверяет условия ачивок нужного типа и выдаёт те, что выполнены и ещё не получены.
    /// Вызывать на триггерных событиях (создание сета, завершение сессии и т.д.)
    /// </summary>
    Task CheckAndGrantAsync(Guid userId, AchievementType type);
}
