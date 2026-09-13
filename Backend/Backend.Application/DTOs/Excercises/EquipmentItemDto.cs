namespace Backend.Application.DTOs.Excercises;

public class EquipmentItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? NameRu { get; set; }
    public string? ImageUrl { get; set; }
}
