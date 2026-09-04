using Backend.Application.Services;
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
    private readonly IServiceScopeFactory _scopeFactory;

    public AdminEmbeddingController(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [HttpPost("exercises")]
    public IActionResult PopulateExerciseEmbeddings()
    {
        _ = Task.Run(async () =>
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var svc = scope.ServiceProvider.GetRequiredService<IExerciseEmbeddingService>();
            await svc.PopulateAllAsync();
        });

        return Accepted(new { message = "Embedding population started in background." });
    }
}
