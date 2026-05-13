namespace Backend.Core.Entities.ExerciseRelated;

public class Equipment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public List<ExerciseEquipment> ExerciseEquipments { get; set; } = [];
}
