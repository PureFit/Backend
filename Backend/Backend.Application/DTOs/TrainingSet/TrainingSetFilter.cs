using Backend.Core.Enums;

namespace Backend.Application.DTOs.TrainingSet;

public class TrainingSetFilter
{
    public Guid? CreatedByUserId { get; set; }
    public bool OnlyMine { get; set; } = false;
    public string? SearchQuery { get; set; }
    public SetAccessType? AccessType { get; set; }
    public TrainingType? TrainingType { get; set; }
    public BodyPartFocus? BodyPartFocus { get; set; }

    // сортировка: сеты где эти мышцы больше всего задействованы идут первыми
    public List<string>? SortByMuscles { get; set; }

    // исключить сеты где эти мышцы задействованы (травма, реабилитация)
    public List<string>? ExcludeMuscleNames { get; set; }

    // исключить сеты с таким фокусом на часть тела
    public List<BodyPartFocus>? ExcludeBodyPartFocus { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
