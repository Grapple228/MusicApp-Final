using Music.Shared.Common;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;

namespace MusicClient.Api.Artists;

public abstract class ArtistsApiBase : MediaApiBase<MediaArtistDto>,  IArtistsApi
{
    protected ArtistsApiBase(IApiClient client) : base(client, ServiceNames.Artists)
    {
    }
    
    public virtual async Task<ArtistDto> Get(Guid artistId)
    {
        var request = CreateRequest(additional:$"{artistId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<ArtistDto>();
    }
    
    public virtual async Task<PageResult<ArtistDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}{(searchQuery == null ? "" : $"?{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<ArtistDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<ArtistDto>> GetAll(long count = 30, string? searchQuery = null)
    {
        var request = CreateRequest(additional: $"?{nameof(count)}={count}{(searchQuery == null ? "" : $"&{nameof(searchQuery)}={searchQuery}")}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<ArtistDto>>();
    }
}