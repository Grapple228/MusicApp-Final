using System.Net;

namespace Music.Services.Exceptions.Exceptions;

public class NotFoundException : ExceptionBase
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message)
    {
    }
}