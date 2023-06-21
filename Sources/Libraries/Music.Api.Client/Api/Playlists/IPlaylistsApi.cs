using Music.Shared.Common;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Requests.Playlists;

namespace MusicClient.Api.Playlists;

public interface IPlaylistsApi : IMediaApi<MediaPlaylistDto>
{
    Task<PlaylistDto> Get(Guid playlistId);
    Task<PageResult<PlaylistDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<PlaylistDto>> GetAll(long count = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<PlaylistDto>> GetAllUser();
    Task<PlaylistDto> Create(PlaylistCreateRequest playlistCreateRequest);
    Task Delete(Guid playlistId);
    Task ChangeTitle(Guid playlistId, string title);
    Task ChangePrivacy(Guid playlistId, bool isPublic);
    Task AddTrack(Guid playlistId, Guid trackId);
    Task RemoveTrack(Guid playlistId, Guid trackId);
    Task AddTracks(Guid playlistId, Guid mediaId, AddToPlaylist addToPlaylist);
}