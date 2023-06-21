namespace MusicClient.Client;

public sealed class ApiClient : ApiClientBase
{
    public ApiClient(string gatewayPath, string accessToken, string refreshToken) : 
        base(gatewayPath, accessToken, refreshToken)
    {
    }
}