namespace Backend.Application.DTOs.Excercises;

public class MuscleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
}
