using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Albums;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Albums;

public abstract class AlbumsApiBase : MediaApiBase<MediaAlbumDto>, IAlbumsApi
{
    protected AlbumsApiBase(IApiClient client) : base(client, ServiceNames.Albums)
    {
    }

    public virtual async Task<AlbumDto> Get(Guid albumId)
    {
        var request = CreateRequest(additional: $"{albumId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<AlbumDto>();
    }

    public virtual async Task<PageResult<AlbumDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}{(searchQuery == null ? "" : $"?{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<AlbumDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<AlbumDto>> GetAll(long count = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"?{nameof(count)}={count}{(searchQuery == null ? "" : $"&{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<AlbumDto>>();
    }

    public virtual async Task<IReadOnlyCollection<AlbumDto>> GetAllUser()
    {
        var request = CreateRequest(additional: "my");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<AlbumDto>>();
    }

    public virtual async Task<AlbumDto> Create(AlbumCreateRequest albumCreateRequest, Guid? artistId = null)
    {
        var request = CreateRequest(Method.Post, additional: $"{(artistId == null ? "" : artistId)}");
        request.AddJsonBody(albumCreateRequest);
        var response = await ExecuteRequest(request);
        return response.Deserialize<AlbumDto>();
    }

    public virtual async Task Delete(Guid albumId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{albumId}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeTitle(Guid albumId, string title)
    {
        var request = CreateRequest(Method.Put, additional: $"{albumId}/{title}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePublicity(Guid albumId, bool isPublic)
    {
        var request = CreateRequest(Method.Put, additional: $"{albumId}/{isPublic}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePublicationDate(Guid albumId, DateOnly publicationDate)
    {
        var request = CreateRequest(Method.Put, additional: $"{albumId}/{publicationDate}");
        await ExecuteRequest(request);
    }

    public virtual async Task AddTrack(Guid albumId, Guid trackId)
    {
        var request = CreateRequest(Method.Post, additional: $"{albumId}/tracks/{trackId}");
        await ExecuteRequest(request);
    }

    public virtual async Task RemoveTrack(Guid albumId, Guid trackId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{albumId}/tracks/{trackId}");
        await ExecuteRequest(request);
    }

    public virtual async Task AddArtist(Guid albumId, Guid artistId)
    {
        var request = CreateRequest(Method.Post, additional: $"{albumId}/artists/{artistId}");
        await ExecuteRequest(request);
    }

    public virtual async Task RemoveArtist(Guid albumId, Guid artistId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{albumId}/artists/{artistId}");
        await ExecuteRequest(request);
    }
}