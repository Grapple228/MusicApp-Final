using Music.Shared.DTOs.Streaming;

namespace MusicClient.Api.Streaming;

public interface IStreamingApi
{
    Task<MemoryStream> GetStream(Guid trackId);
    Task<RoomDto> CreateRoom();
}