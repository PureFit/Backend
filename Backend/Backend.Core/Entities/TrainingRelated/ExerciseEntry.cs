using System.ComponentModel.DataAnnotations.Schema;
using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    public class ExerciseEntry
    {
        public Guid Id { get; set; }

        public string ExerciseId { get; set; } = null!;

        public int Order { get; set; }
        public MeasureType MeasureType { get; set; }

        public int? Reps { get; set; }
        public int? DurationSeconds { get; set; }
        public float? DistanceMeters { get; set; }

        [Column(TypeName = "jsonb")]
        public Dictionary<string, float>? Parameters { get; set; } // weight_kg, speed_kmh, incline, watts и т.д.

        // Данные о мышцах берутся с фронта при добавлении упражнения (из ExerciseDetailsDto)
        [Column(TypeName = "jsonb")]
        public List<string> TargetMuscles { get; set; } = [];

        [Column(TypeName = "jsonb")]
        public List<string> SecondaryMuscles { get; set; } = [];

        [Column(TypeName = "jsonb")]
        public List<string> BodyParts { get; set; } = [];

        public int RestAfterCurrentEntrySeconds { get; set; }

        // Если заполнено — интервальное упражнение. Reps/DurationSeconds/Parameters выше игнорируются.
        public List<ExerciseInterval> Intervals { get; set; } = [];

        public SetBlock SetBlock { get; set; } = null!;
        public Guid SetBlockId { get; set; }
    }
}
