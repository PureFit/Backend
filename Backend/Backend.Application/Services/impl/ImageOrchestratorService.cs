using Backend.Application.Common.Exceptions;
using Backend.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services.impl;

public class ImageOrchestratorService : IImageOrchestratorService
{
    private readonly IImageRetrieverService _lowLevelRetriever;
    private readonly ILogger<ImageOrchestratorService> _logger;

    public ImageOrchestratorService(
        IImageRetrieverService lowLevelRetriever,
        ILogger<ImageOrchestratorService> logger)
    {
        _lowLevelRetriever = lowLevelRetriever;
        _logger = logger;
    }

    public async Task<string?> UploadFileAsync(IFormFile? file, Guid identifier)
    {
        if (file is null || file.Length == 0) return null;

        try
        {
            var fileName = $"avatars/{identifier}";

            using var stream = file.OpenReadStream();
            return await _lowLevelRetriever.UploadAvatarAsync(stream, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Image orchestration failed for {Identifier}", identifier);
            throw new ImageUploadException("Could not process image upload", ex);
        }
    }

    public async Task DeleteFileAsync(string? photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl)) return;

        try
        {
            var publicId = _lowLevelRetriever.ExtractPublicId(photoUrl);

            if (string.IsNullOrEmpty(publicId)) return;

            await _lowLevelRetriever.DeleteAvatarAsync(publicId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image from cloud: {Url}", photoUrl);
        }
    }
}
