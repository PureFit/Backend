namespace Backend.Core.Entities.TrainingRelated
{
    public class ExerciseInterval
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int? Reps { get; set; }
        public int? DurationSeconds { get; set; }
        public float? DistanceMeters { get; set; }
        public float? WeightKg { get; set; }
        public float? SpeedKmh { get; set; }

        public Guid ExerciseEntryId { get; set; }
        public ExerciseEntry ExerciseEntry { get; set; } = null!;
    }
}
