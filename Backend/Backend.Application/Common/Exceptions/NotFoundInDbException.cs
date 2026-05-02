namespace Backend.Application.Common.Exceptions;

public class NotFoundInDbException : Exception
{
    public NotFoundInDbException() : base("Entity not found in database") { }
    public NotFoundInDbException(string message) : base(message) { }
    public NotFoundInDbException(string message, Exception inner) : base(message, inner) { }
}
