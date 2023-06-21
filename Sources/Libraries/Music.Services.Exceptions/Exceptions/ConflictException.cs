using System.Net;

namespace Music.Services.Exceptions.Exceptions;

public class ConflictException : ExceptionBase
{
    public ConflictException(string message) : base(HttpStatusCode.Conflict, message)
    {
    }
}