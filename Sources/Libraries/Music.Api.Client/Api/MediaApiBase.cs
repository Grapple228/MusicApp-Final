using Music.Shared.Common;
using Music.Shared.DTOs.Media.Enums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;
using MusicClient.Client;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api;

public abstract class MediaApiBase<TMediaDto> : ApiBase, IMediaApi<TMediaDto> where TMediaDto : IModel
{
    protected MediaApiBase(IApiClient client, string serviceName) : base(client, serviceName)
    {
    }
    
    public virtual async Task<TMediaDto> GetMedia(Guid mediaId)
    {
        var request = CreateRequest(additional:$"media/{mediaId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<TMediaDto>();
    }

    public virtual async Task ChangeMedia(Guid mediaId, MediaCreateRequest createRequest)
    {
        var request = CreateRequest(Method.Post, additional:$"media/{mediaId}");
        request.AddJsonBody(createRequest);
        await ExecuteRequest(request);
    }

    public virtual async Task<IReadOnlyCollection<TMediaDto>> GetAllMedia(MediaFilterEnum filter = MediaFilterEnum.All)
    {
        var request = CreateRequest(additional:$"media/{filter.GetPath()}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<TMediaDto>>();
    }
    
    public virtual async Task<IReadOnlyCollection<TMediaDto>> GetAllUserMedia()
    {
        var request = CreateRequest(additional:$"media/user");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<TMediaDto>>();
    }

    public virtual async Task<PageResult<TMediaDto>> GetAllMediaPaged(long pageNumber, long countPerPage, MediaFilterEnum filter = MediaFilterEnum.All)
    {
        var request = CreateRequest(additional: $"media/{filter.GetPath()}/{pageNumber}-{countPerPage}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<TMediaDto>>();
    }

    public virtual async Task<PageResult<TMediaDto>> GetAllUserMediaPaged(long pageNumber, long countPerPage)
    {
        var request = CreateRequest(additional: $"media/{pageNumber}-{countPerPage}/user");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<TMediaDto>>();
    }

    public virtual async Task<IReadOnlyCollection<MediaTrackDto>> GetTracks(Guid mediaId)
    {
        var request = CreateRequest(additional:$"media/{mediaId}/Tracks");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<MediaTrackDto>>();
    }
}