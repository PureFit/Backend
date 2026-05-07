using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class UpdateSetBlockExerciseEntryRequest
{
    public Guid SetId { get; set; }
    public Guid BlockId { get; set; }
    public Guid EntryId { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public Dictionary<string, float>? Parameters { get; set; }
    public int? RestAfterCurrentEntrySeconds { get; set; }
    public int? Order { get; set; }
    public MeasureType? MeasureType { get; set; }
}
