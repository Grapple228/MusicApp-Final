using System.Net;

namespace Music.Services.Exceptions.Exceptions;

public class ForbiddenException : ExceptionBase
{
    public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message)
    {
    }
}