using Backend.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Admin-only endpoint для первичной генерации эмбеддингов упражнений.
/// Вызвать один раз после деплоя / после массового добавления упражнений.
/// </summary>
[ApiController]
[Route("api/admin/embeddings")]
public class AdminEmbeddingController : ControllerBase
{
    [HttpPost("exercises")]
    public IActionResult PopulateExerciseEmbeddings()
    {
        BackgroundJob.Enqueue<IExerciseEmbeddingService>(svc => svc.PopulateAllAsync(CancellationToken.None));
        return Accepted(new { message = "Embedding population started in background." });
    }
}
