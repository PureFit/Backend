using Backend.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Backend.Controllers;

[ApiController]
[Route("api/admin/embeddings")]
public class AdminEmbeddingController : ControllerBase
{
    private readonly string _adminKey;

    public AdminEmbeddingController(IConfiguration configuration)
    {
        _adminKey = configuration["AdminSettings:SecretKey"] ?? throw new InvalidOperationException("AdminSettings:SecretKey not configured.");
    }

    [HttpPost("exercises")]
    public IActionResult PopulateExerciseEmbeddings([FromHeader(Name = "X-Admin-Key")] string? adminKey)
    {
        if (adminKey != _adminKey)
            return Unauthorized();

        BackgroundJob.Enqueue<IExerciseEmbeddingService>(svc => svc.PopulateAllAsync(CancellationToken.None));
        return Accepted(new { message = "Embedding population started in background." });
    }

    [HttpPost("translate-exercises")]
    public IActionResult TranslateExerciseNames([FromHeader(Name = "X-Admin-Key")] string? adminKey)
    {
        if (adminKey != _adminKey)
            return Unauthorized();

        BackgroundJob.Enqueue<IExerciseTranslationService>(svc => svc.PopulateNameRuAsync(CancellationToken.None));
        return Accepted(new { message = "Translation started in background." });
    }

    [HttpPost("translate-metadata")]
    public IActionResult TranslateMetadata([FromHeader(Name = "X-Admin-Key")] string? adminKey)
    {
        if (adminKey != _adminKey)
            return Unauthorized();

        BackgroundJob.Enqueue<IMetadataTranslationService>(svc => svc.PopulateAsync(CancellationToken.None));
        return Accepted(new { message = "Metadata translation started in background." });
    }
}
