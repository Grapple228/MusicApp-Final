namespace MusicClient.Exceptions;

public class InvalidRefreshTokenException : ExceptionBase
{
    private const string DefaultMessage = "Invalid refresh token";
    
    public InvalidRefreshTokenException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }
}