using Music.Shared.Common;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Requests.Playlists;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Playlists;

public enum AddToPlaylist
{
    Artist,
    Album,
    Playlist,
    Track
}

public abstract class PlaylistsApiBase : MediaApiBase<MediaPlaylistDto>, IPlaylistsApi
{
    protected PlaylistsApiBase(IApiClient client) : base(client, ServiceNames.Playlists)
    {
    }

    public virtual async Task<PlaylistDto> Get(Guid playlistId)
    {
        var request = CreateRequest(additional:$"{playlistId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PlaylistDto>();
    }

    public virtual async Task<PageResult<PlaylistDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}{(searchQuery == null ? "" : $"?{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<PlaylistDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<PlaylistDto>> GetAll(long count = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"?{nameof(count)}={count}{(searchQuery == null ? "" : $"&{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<PlaylistDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<PlaylistDto>> GetAllUser()
    {
        var request = CreateRequest(additional: "my");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<PlaylistDto>>();
    }

    public virtual async Task<PlaylistDto> Create(PlaylistCreateRequest playlistCreateRequest)
    {
        var request = CreateRequest(Method.Post);
        request.AddJsonBody(playlistCreateRequest);
        var response = await ExecuteRequest(request);
        return response.Deserialize<PlaylistDto>();
    }

    public virtual async Task Delete(Guid playlistId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{playlistId}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeTitle(Guid playlistId, string title)
    {
        var request = CreateRequest(Method.Put, additional: $"{playlistId}/{title}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePrivacy(Guid playlistId, bool isPublic)
    {
        var request = CreateRequest(Method.Put, additional: $"{playlistId}/{isPublic}");
        await ExecuteRequest(request);
    }

    public virtual async Task AddTrack(Guid playlistId, Guid trackId)
    {
        var request = CreateRequest(Method.Post, additional: $"{playlistId}/tracks/{trackId}");
        await ExecuteRequest(request);
    }

    public virtual async Task RemoveTrack(Guid playlistId, Guid trackId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{playlistId}/tracks/{trackId}");
        await ExecuteRequest(request);
    }

    public virtual async Task AddTracks(Guid playlistId, Guid mediaId, AddToPlaylist addToPlaylist)
    {
        var request = CreateRequest(Method.Post, additional: $"{playlistId}/Tracks/{addToPlaylist}/{mediaId}");
        await ExecuteRequest(request);
    }
}