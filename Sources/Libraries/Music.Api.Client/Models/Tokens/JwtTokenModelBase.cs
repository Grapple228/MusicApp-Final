namespace MusicClient.Models.Tokens;

public delegate void TokenChanged();

public abstract class JwtTokenModelBase : IJwtTokenModel
{
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    protected JwtTokenModelBase(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }


    public virtual void ChangeToken(string jwtAccessToken, string jwtRefreshToken)
    {
        AccessToken = jwtAccessToken;
        RefreshToken = jwtRefreshToken;
        Changed?.Invoke(this);
    }

    public event TokenChangedDelegate? Changed;
}