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
}
