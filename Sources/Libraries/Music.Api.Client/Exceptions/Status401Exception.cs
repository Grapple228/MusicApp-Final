namespace MusicClient.Exceptions;

public class Status401Exception : ExceptionBase
{
    private const string DefaultMessage = "Unathorized";
    
    public Status401Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}