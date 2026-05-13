using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core.Entities.ExerciseRelated;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? GifUrl { get; set; }
    public string? Overview { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> Instructions { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public List<string> Tips { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public List<string> Variations { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public List<string> Keywords { get; set; } = [];

    public string Category { get; set; } = null!;

    // Для AllowsWeight упражнений: норма веса относительно веса тела юзера.
    // Жим лёжа = 0.7 (70% веса тела — средний рабочий вес).
    public bool AllowsWeight { get; set; }
    public float? BaseWeightBodyRatio { get; set; }

    // Обычно 1 тип, упражнения вроде box jumps имеют 2 (RepsBased + DurationOnly)
    public List<ExerciseType> ExerciseTypes { get; set; } = [];

    public List<ExerciseMuscle> ExerciseMuscles { get; set; } = [];
    public List<ExerciseEquipment> ExerciseEquipments { get; set; } = [];
    public List<ExerciseBodyPart> ExerciseBodyParts { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public List<string> RelatedExerciseIds { get; set; } = [];
}
