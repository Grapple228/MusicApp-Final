namespace MusicClient.Exceptions;

public class InvalidAuthorizationDataException : ExceptionBase
{
    private const string DefaultMessage = "Invalid authorization data";

    public InvalidAuthorizationDataException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }

}