using Backend.Core.Entities.TrainingRelated;

namespace Backend.Core.Entities
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public Sex Sex { get; set; }
        public Level Level { get; set; }
        public decimal WeightKg { get; set; }
        public decimal HeightCm { get; set; }
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Накопленная нагрузка по мышцам и частям тела за все тренировки.
        /// Обновляется инкрементально после каждой сессии (опция B).
        /// Включает как плановые, так и свободные тренировки.
        /// </summary>
        public List<UserWorkloadStat> WorkloadStats { get; set; } = [];

        public Guid? CurrentPlanId { get; set; }
        public AIPlan? CurrentPlan { get; set; }
        public List<AIPlan> Plans { get; set; } = [];

        // Все тренировки пользователя — и плановые, и свободные.
        // Когда плана нет — сессии хранятся только здесь.
        // Когда план есть — сессии дублируются в WeekPlan.InternalTrainings для контекста недели.
        public List<TrainingSession> Sessions { get; set; } = [];

        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    public enum Sex
    {
        Man,
        Woman
    }

    public enum Level
    {
        Beginner,
        Returning,
        Intermediate,
        Advanced,
        Elite
    }
}
