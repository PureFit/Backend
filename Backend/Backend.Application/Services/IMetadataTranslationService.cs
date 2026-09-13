namespace Backend.Application.Services;

public interface IMetadataTranslationService
{
    Task PopulateAsync(CancellationToken ct = default);
}
