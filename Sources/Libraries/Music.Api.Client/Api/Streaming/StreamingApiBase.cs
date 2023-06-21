using Music.Shared.DTOs.Streaming;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Streaming;

public abstract class StreamingApiBase : ApiBase, IStreamingApi
{
    protected StreamingApiBase(IApiClient client) : base(client, "streaming")
    {
    }
    
    public virtual async Task<MemoryStream> GetStream(Guid trackId)
    {
        var request = CreateRequest(additional: $"{trackId}");
        var response = await ExecuteRequest(request);
        return new MemoryStream(response.RawBytes!);
    }

    public virtual async Task<RoomDto> CreateRoom()
    {
        var request = CreateRequest(Method.Post, additional: "Rooms");
        var response = await ExecuteRequest(request);
        return response.Deserialize<RoomDto>();
    }
    
    public virtual async Task<RoomDto> DeleteRoom()
    {
        var request = CreateRequest(Method.Delete, additional: "Rooms");
        var response = await ExecuteRequest(request);
        return response.Deserialize<RoomDto>();
    }
}