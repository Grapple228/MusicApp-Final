using MusicClient.Client;

namespace MusicClient.Api.Genres;

public class GenreApi : GenreApiBase
{
    public GenreApi(IApiClient client) : base(client)
    {
    }
}