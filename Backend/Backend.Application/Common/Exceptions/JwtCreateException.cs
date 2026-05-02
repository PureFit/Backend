namespace Backend.Application.Common.Exceptions;

public class JwtCreateException : Exception
{
    public JwtCreateException(string message) : base(message) { }
    public JwtCreateException(string message, Exception inner) : base(message, inner) { }
}
