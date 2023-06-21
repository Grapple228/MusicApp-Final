using System.Net;

namespace Music.Services.Exceptions.Exceptions;

public class BadRequestException : ExceptionBase
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message)
    {
    }
}