namespace MusicClient.Exceptions;

public class InvalidUrlException : ExceptionBase
{
    private const string DefaultMessage = "Invalid url";
    
    public InvalidUrlException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}