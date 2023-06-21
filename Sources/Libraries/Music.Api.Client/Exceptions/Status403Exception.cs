namespace MusicClient.Exceptions;

public class Status403Exception : ExceptionBase
{
    private const string DefaultMessage = "Forbidden";
    
    public Status403Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}