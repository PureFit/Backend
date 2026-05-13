using Backend.Core.Entities.ExerciseRelated;

namespace Backend.Core.Entities.TrainingRelated
{
    public class ExerciseEntry
    {
        public Guid Id { get; set; }
        public int Order { get; set; }

        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        // Выбранный тип измерения (для упражнений с несколькими типами юзер выбирает сам)
        public Guid ExerciseTypeId { get; set; }
        public ExerciseType ExerciseType { get; set; } = null!;

        public int? Reps { get; set; }
        public int? DurationSeconds { get; set; }
        public float? DistanceMeters { get; set; }
        public float? WeightKg { get; set; }
        public float? SpeedKmh { get; set; }

        public int RestAfterCurrentEntrySeconds { get; set; }

        public List<ExerciseInterval> Intervals { get; set; } = [];

        public SetBlock SetBlock { get; set; } = null!;
        public Guid SetBlockId { get; set; }
    }
}
