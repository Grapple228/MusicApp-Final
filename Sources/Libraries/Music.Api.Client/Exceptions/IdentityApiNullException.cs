namespace MusicClient.Exceptions;

public class IdentityApiNullException : ExceptionBase
{
    private const string DefaultMessage = "IIdentity Api is null!";

    public IdentityApiNullException(string message = DefaultMessage) : base(message == "" ? DefaultMessage : message)
    {
    }

}