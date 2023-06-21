using MusicClient.Client;

namespace MusicClient.Api.Artists;

public class ArtistsApi : ArtistsApiBase
{
    public ArtistsApi(IApiClient client) : base(client)
    {
    }
}