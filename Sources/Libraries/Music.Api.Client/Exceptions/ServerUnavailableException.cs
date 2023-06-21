namespace MusicClient.Exceptions;

public class ServerUnavailableException : ExceptionBase
{
    private const string DefaultMessage = "Server is unavailable";
    
    public ServerUnavailableException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}