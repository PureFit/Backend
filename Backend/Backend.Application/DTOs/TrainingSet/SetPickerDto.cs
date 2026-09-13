namespace Backend.Application.DTOs.TrainingSet;

public class SetPickerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<BlockPickerDto> SetBlocks { get; set; } = [];
}

public class BlockPickerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Order { get; set; }
    public int ExercisesCount { get; set; }
}
