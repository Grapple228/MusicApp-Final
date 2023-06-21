using Music.Shared.Identity.Jwt;
using MusicClient.Api.Albums;
using MusicClient.Api.Artists;
using MusicClient.Api.Genres;
using MusicClient.Api.Identity;
using MusicClient.Api.Images;
using MusicClient.Api.Playlists;
using MusicClient.Api.Streaming;
using MusicClient.Api.Tracks;
using MusicClient.Api.Users;
using MusicClient.Models.Tokens;
using RestSharp;

namespace MusicClient.Client;

public interface IApiClient
{
    IJwtTokenModel Token { get; }
    Task<LoginDto> RefreshToken();
    
    IRestClient RestClient { get; } 
    IIdentityApi IdentityApi { get; }
    IAlbumsApi AlbumsApi { get; }
    IArtistsApi ArtistsApi { get; }
    IGenresApi GenresApi { get; }
    IPlaylistsApi PlaylistsApi { get; }
    ITracksApi TracksApi { get; }
    IUsersApi UsersApi { get; }
    IStreamingApi StreamingApi { get; }
    IImagesApi ImagesApi { get; }
    void SetIdentityApi(IIdentityApi identityApi);
    void SetUsersApi(IUsersApi usersApi);
    void SetGenresApi(IGenresApi genresApi);
    void SetArtistsApi(IArtistsApi artistsApi);
    void SetAlbumsApi(IAlbumsApi albumsApi);
    void SetPlaylistsApi(IPlaylistsApi playlistsApi);
    void SetTracksApi(ITracksApi tracksApi);
    void SetStreamingApi(IStreamingApi streamingApi);
    void SetImagesApi(IImagesApi imagesApi);
}