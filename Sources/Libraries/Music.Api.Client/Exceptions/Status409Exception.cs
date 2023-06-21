namespace MusicClient.Exceptions;

public class Status409Exception : ExceptionBase
{
    private const string DefaultMessage = "Conflict";
    
    public Status409Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}