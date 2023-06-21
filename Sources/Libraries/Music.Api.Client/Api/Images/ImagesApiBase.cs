using Music.Shared.Files.Common;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Exceptions;
using MusicClient.Helpers;
using RestSharp;

namespace MusicClient.Api.Images;

public abstract class ImagesApiBase : ApiBase, IImagesApi
{
    protected ImagesApiBase(IApiClient client) : base(client, ServiceNames.Images)
    {
    }

    public virtual async Task ChangeTrack(Guid trackId, string filename)
    {
        var request = CreateRequest(Method.Post, additional: $"{ServiceNames.Tracks}/{trackId}");
        request.AddFile(filename);
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeAlbum(Guid albumId, string filename)
    {
        var request = CreateRequest(Method.Post, additional: $"{ServiceNames.Albums}/{albumId}");
        request.AddFile(filename);
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeArtist(Guid artistId, string filename)
    {
        var request = CreateRequest(Method.Post, additional: $"{ServiceNames.Artists}/{artistId}");
        request.AddFile(filename);
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePlaylist(Guid playlistId, string filename)
    {
        var request = CreateRequest(Method.Post, additional: $"{ServiceNames.Playlists}/{playlistId}");
        request.AddFile(filename);
        await ExecuteRequest(request);
    }

    public virtual async Task ChangeUser(Guid userId, string filename)
    {
        var request = CreateRequest(Method.Post, additional: $"{ServiceNames.Users}/{userId}");
        request.AddFile(filename);
        await ExecuteRequest(request);
    }

    public virtual async Task<byte[]> GetImage(string serviceName, Guid modelId, ImageSizeEnum size)
    {
        var request = CreateRequest(additional: $"{serviceName}/{modelId}/{size}");
        var response = await ExecuteRequest(request);
        return response.RawBytes ?? throw new ServerUnavailableException("");
    }
}