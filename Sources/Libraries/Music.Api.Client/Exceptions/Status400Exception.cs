namespace MusicClient.Exceptions;

public class Status400Exception : ExceptionBase
{
    private const string DefaultMessage = "Bad request";
    
    public Status400Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}