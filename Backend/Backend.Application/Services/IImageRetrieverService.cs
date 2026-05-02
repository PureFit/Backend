namespace Backend.Application.Services;

public interface IImageRetrieverService
{
    Task<string> UploadAvatarAsync(Stream imageStream, string fileName);
    Task<bool> DeleteAvatarAsync(string publicId);
    string? ExtractPublicId(string fileUrl);
}
