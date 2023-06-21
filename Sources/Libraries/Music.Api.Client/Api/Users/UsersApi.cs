using MusicClient.Client;

namespace MusicClient.Api.Users;

public class UsersApi : UsersApiBase
{
    public UsersApi(IApiClient client) : base(client)
    {
    }
}