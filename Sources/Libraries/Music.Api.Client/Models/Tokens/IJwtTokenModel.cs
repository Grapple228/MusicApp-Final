namespace MusicClient.Models.Tokens;

public interface IJwtTokenModel : IToken
{
    void ChangeToken(string jwtAccessToken, string jwtRefreshToken);
    string AccessToken { get; }
    string RefreshToken { get; }
}