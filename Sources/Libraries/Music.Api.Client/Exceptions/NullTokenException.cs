namespace MusicClient.Exceptions;

public class NullTokenException : ExceptionBase
{
    private const string DefaultMessage = "Token is empty";
    
    public NullTokenException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}