using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core.Entities.TrainingRelated
{
    public class ExerciseInterval
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int DurationSeconds { get; set; }
        public int? Reps { get; set; }

        [Column(TypeName = "jsonb")]
        public Dictionary<string, float>? Parameters { get; set; } // speed_kmh, incline, watts, weight_kg и т.д.

        public Guid ExerciseEntryId { get; set; }
        public ExerciseEntry ExerciseEntry { get; set; } = null!;
    }
}
