namespace Backend.Application.Services;

public interface IExerciseTranslationService
{
    Task PopulateNameRuAsync(CancellationToken ct = default);
}
