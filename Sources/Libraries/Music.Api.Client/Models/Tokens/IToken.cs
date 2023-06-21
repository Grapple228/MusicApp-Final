namespace MusicClient.Models.Tokens;

public delegate void TokenChangedDelegate(IToken token);

public interface IToken
{
    event TokenChangedDelegate Changed;
}