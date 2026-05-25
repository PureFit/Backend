namespace Backend.Application.Services;

public interface IImageRetrieverService
{
    Task<string> UploadAvatarAsync(Stream imageStream, string fileName);
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder);
    Task<bool> DeleteAvatarAsync(string publicId);
    string? ExtractPublicId(string fileUrl);
}
