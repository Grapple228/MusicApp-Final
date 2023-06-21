using MusicClient.Client;

namespace MusicClient.Api.Playlists;

public class PlaylistsApi : PlaylistsApiBase
{
    public PlaylistsApi(IApiClient client) : base(client)
    {
    }
}