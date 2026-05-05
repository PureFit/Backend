using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    public class TrainingSession
    {
        public Guid Id { get; set; }

        public Guid UserInfoId { get; set; }
        public UserInfo UserInfo { get; set; } = null!;

        public SessionStatus Status { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public string? Notes { get; set; }

        public Guid? PlanTrainingId { get; set; }
        public PlanTraining? PlanTraining { get; set; }

        // Используется только для свободных тренировок (без плана).
        public Guid? TrainingSetId { get; set; }
        public TrainingSet? TrainingSet { get; set; }
    }
}
