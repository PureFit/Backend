namespace Backend.Application.DTOs.Plan;

/// <summary>
/// Компактное представление упражнения для AI промпта.
/// Формат строки: id|name|bodyParts|muscles|equipment|measure
/// </summary>
public class ExerciseBrief
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<string> BodyParts { get; set; } = [];
    public List<string> Muscles { get; set; } = [];
    public List<string> Equipment { get; set; } = [];
    public string Measure { get; set; } = null!;

    public string ToCompactString() =>
        $"{Id}|{Name}|{string.Join(",", BodyParts)}|{string.Join(",", Muscles)}|{string.Join(",", Equipment)}|{Measure}";
}
