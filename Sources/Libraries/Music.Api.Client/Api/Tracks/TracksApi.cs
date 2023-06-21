using MusicClient.Client;

namespace MusicClient.Api.Tracks;

public class TracksApi : TracksApiBase
{
    public TracksApi(IApiClient client) : base(client)
    {
    }
}