using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Requests.Genres;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Genres;

public abstract class GenreApiBase : ApiBase, IGenresApi
{
    protected GenreApiBase(IApiClient client) : base(client, ServiceNames.Genres)
    {
    }

    public virtual async Task<GenreDto> Get(Guid genreId)
    {
        var request = CreateRequest(additional:$"{genreId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<GenreDto>();
    }

    public virtual async Task<PageResult<GenreDto>> GetAllPaged(long pageNumber, long countPerPage)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<GenreDto>>();
    }

    public virtual async Task<IReadOnlyCollection<GenreDto>> GetAll()
    {
        var request = CreateRequest();
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<GenreDto>>();
    }

    public virtual async Task<GenreDto> Create(GenreCreateRequest createRequest)
    {
        var request = CreateRequest(Method.Post);
        request.AddJsonBody(createRequest);
        var response = await ExecuteRequest(request);
        return response.Deserialize<GenreDto>();
    }

    public virtual async Task<GenreDto> Update(Guid genreId, GenreUpdateRequest updateRequest)
    {
        var request = CreateRequest(Method.Put, additional: $"{genreId}");
        request.AddJsonBody(updateRequest);
        var response = await ExecuteRequest(request);
        return response.Deserialize<GenreDto>();
    }

    public virtual async Task Delete(Guid genreId)
    {
        var request = CreateRequest(Method.Delete, additional: $"{genreId}");
        await ExecuteRequest(request);
    }
}