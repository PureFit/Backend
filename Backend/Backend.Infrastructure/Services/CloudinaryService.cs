using Backend.Application.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Backend.Application.Common;

namespace Backend.Infrastructure.Services;

public class CloudinaryService : IImageRetrieverService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(
        ILogger<CloudinaryService> logger,
        IOptions<CloudinarySettings> cloudinarySettings)
    {
        _logger = logger;
        _settings = cloudinarySettings.Value;

        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadAvatarAsync(Stream imageStream, string fileName)
    {
        try
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, imageStream),
                Folder = "avatars",
                Overwrite = true,
                Invalidate = true,
                PublicId = fileName,
                Transformation = new Transformation()
                    .Width(_settings.TransformationOptions.Width)
                    .Height(_settings.TransformationOptions.Height)
                    .Crop(_settings.TransformationOptions.Crop)
                    .Quality(_settings.TransformationOptions.Quality)
                    .FetchFormat(_settings.TransformationOptions.Format)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Avatar uploaded successfully: {PublicId}", uploadResult.PublicId);
                return uploadResult.SecureUrl.ToString();
            }

            _logger.LogError("Avatar upload failed: {Error}", uploadResult.Error?.Message);
            throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading avatar for file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteAvatarAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting avatar: {PublicId}", publicId);
            return false;
        }
    }

    public string? ExtractPublicId(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var segments = uri.AbsolutePath.Split('/');

            var folderIndex = Array.IndexOf(segments, "avatars");
            if (folderIndex == -1) return null;

            var fileNameWithExt = segments.Last();
            var fileName = Path.GetFileNameWithoutExtension(fileNameWithExt);

            return $"avatars/{fileName}";
        }
        catch { return null; }
    }
}
