using Music.Shared.Common;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Tracks;

public abstract class TracksApiBase : MediaApiBase<MediaTrackDto>, ITracksApi
{
    protected TracksApiBase(IApiClient client) : base(client, ServiceNames.Tracks)
    {
    }

    public virtual async Task<TrackDto> Get(Guid trackId)
    {
        var request = CreateRequest(additional:$"{trackId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<TrackDto>();
    }

    public virtual async Task<PageResult<TrackDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}{(searchQuery == null ? "" : $"?{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<TrackDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<TrackDto>> GetAll(long count = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"?{nameof(count)}={count}{(searchQuery == null ? "" : $"&{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<TrackDto>>();
    }

    public virtual async Task<IReadOnlyCollection<TrackDto>> GetAllUser()
    {
        var request = CreateRequest(additional: "my");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<TrackDto>>();
    }

    public virtual async Task<TrackDto> Create(string title, IReadOnlyCollection<Guid> genres, DateOnly publicationDate, string fileName, Guid? artistId = null)
    {
        var request = CreateRequest(Method.Post, additional: $"{(artistId == null ? "" : artistId)}?Title={title}" +
                                                             $"&PublicationDate={publicationDate.Year}-{publicationDate.Month}-{publicationDate.Day}" +
                                                             (genres.Count == 0 ? "" : $"&Genres={string.Join("&Genres=", genres)}"));
        request.AddFile(fileName);
        var response = await ExecuteRequest(request);
        return response.Deserialize<TrackDto>();
    }

    public virtual async Task Delete(Guid trackId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{trackId}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeTitle(Guid trackId, string title)
    {
        var request = CreateRequest(Method.Put, additional: $"{trackId}/{title}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePrivacy(Guid trackId, bool isPublic)
    {
        var request = CreateRequest(Method.Put, additional: $"{trackId}/{isPublic}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePublicationDate(Guid trackId, DateOnly publicationDate)
    {
        var request = CreateRequest(Method.Put, additional: $"{trackId}/{publicationDate}");
        await ExecuteRequest(request);
    }

    public virtual async Task<int> ChangeFile(Guid trackId, string filename)
    {
        var request = CreateRequest(Method.Put, additional: $"{trackId}");
        request.AddFile(filename);
        var response = await ExecuteRequest(request);
        return response.Deserialize<int>();
    }

    public virtual async Task AddGenre(Guid trackId, Guid genreId)
    {
        var request = CreateRequest(Method.Post, additional: $"{trackId}/genres/{genreId}");
        await ExecuteRequest(request);
    }

    public virtual async Task RemoveGenre(Guid trackId, Guid genreId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{trackId}/genres/{genreId}");
        await ExecuteRequest(request);
    }

    public virtual async Task AddArtist(Guid trackId, Guid artistId)
    {
        var request = CreateRequest(Method.Post, additional: $"{trackId}/artists/{artistId}");
        await ExecuteRequest(request);
    }

    public virtual async Task RemoveArtist(Guid trackId, Guid artistId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{trackId}/artists/{artistId}");
        await ExecuteRequest(request);
    }
}