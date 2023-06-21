using MusicClient.Api.Albums;
using MusicClient.Api.Artists;
using MusicClient.Api.Genres;
using MusicClient.Api.Identity;
using MusicClient.Api.Images;
using MusicClient.Api.Playlists;
using MusicClient.Api.Streaming;
using MusicClient.Api.Tracks;
using MusicClient.Api.Users;
using MusicClient.Client;

namespace MusicClient.Helpers;

public static class ApiCreator
{
    public static IIdentityApi GetDefaultIdentityApi(this IApiClient client) => 
        new IdentityApi(client);

    public static IUsersApi GetDefaultUsersApi(this IApiClient client) =>
        new UsersApi(client);

    public static IGenresApi GetDefaultGenresApi(this IApiClient client) =>
        new GenreApi(client);

    public static IArtistsApi GetDefaultArtistsApi(this IApiClient client) =>
        new ArtistsApi(client);
    
    public static IAlbumsApi GetDefaultAlbumsApi(this IApiClient client) =>
        new AlbumsApi(client);
    
    public static IPlaylistsApi GetDefaultPlaylistsApi(this IApiClient client) =>
        new PlaylistsApi(client);

    public static ITracksApi GetDefaultTracksApi(this IApiClient client) =>
        new TracksApi(client);
    
    public static IStreamingApi GetDefaultStreamingApi(this IApiClient client) =>
        new StreamingApi(client);
    
    public static IImagesApi GetDefaultImagesApi(this IApiClient client) =>
        new ImagesApi(client);
}