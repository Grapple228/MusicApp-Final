namespace MusicClient.Models.Tokens;

public class DefaultJwtTokenModel : JwtTokenModelBase
{
    public DefaultJwtTokenModel(string accessToken, string refreshToken) : base(accessToken, refreshToken)
    {
    }
}