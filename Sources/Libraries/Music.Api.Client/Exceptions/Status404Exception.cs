namespace MusicClient.Exceptions;

public class Status404Exception : ExceptionBase
{
    private const string DefaultMessage = "Not found";
    
    public Status404Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}