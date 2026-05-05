namespace Backend.Core.Entities.TrainingRelated
{
    public class PlanTraining
    {
        public Guid Id { get; set; }
        public int TrainingNumber { get; set; }
        public string Description { get; set; } = null!;

        public DateTime StartPlannedDate { get; set; }
        public DateTime EndPlannedDate { get; set; }

        public WeekPlan WeekPlan { get; set; } = null!;
        public Guid WeekPlanId { get; set; }

        // Шаблон тренировки (что делать).
        public TrainingSet TrainingSet { get; set; } = null!;
        public Guid TrainingSetId { get; set; }

        // Факт выполнения (null если ещё не начата).
        public TrainingSession? TrainingSession { get; set; }
    }
}
