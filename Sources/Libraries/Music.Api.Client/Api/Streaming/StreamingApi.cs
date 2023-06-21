using MusicClient.Client;

namespace MusicClient.Api.Streaming;

public class StreamingApi : StreamingApiBase
{
    public StreamingApi(IApiClient client) : base(client)
    {
    }
}