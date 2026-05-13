namespace Backend.Core.Entities.ExerciseRelated;

public class ExerciseEquipment
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}
