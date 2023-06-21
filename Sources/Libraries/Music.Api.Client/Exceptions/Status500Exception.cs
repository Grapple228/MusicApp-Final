namespace MusicClient.Exceptions;

public class Status500Exception : ExceptionBase
{
    private const string DefaultMessage = "Server problem";
    
    public Status500Exception(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}