using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    // Накопленная нагрузка по мышце или части тела за все тренировки пользователя.
    // Обновляется инкрементально после каждой завершённой сессии.
    public class UserWorkloadStat
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;          // название мышцы или части тела
        public WorkloadStatCategory Category { get; set; }
        public float TotalPercent { get; set; }             // накопленный процент нагрузки
        public DateTime LastUpdatedAt { get; set; }

        public Guid UserInfoId { get; set; }
        public UserInfo UserInfo { get; set; } = null!;
    }
}
