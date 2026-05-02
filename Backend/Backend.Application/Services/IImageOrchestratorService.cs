using Microsoft.AspNetCore.Http;

namespace Backend.Application.Services;

public interface IImageOrchestratorService
{
    Task<string?> UploadFileAsync(IFormFile? file, Guid entityId);
    Task DeleteFileAsync(string? fileUrl);
}
