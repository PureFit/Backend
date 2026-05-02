namespace Backend.Application.Common.Exceptions;

public class ImageUploadException : Exception
{
    public ImageUploadException(string message) : base(message) { }
    public ImageUploadException(string message, Exception inner) : base(message, inner) { }
}
