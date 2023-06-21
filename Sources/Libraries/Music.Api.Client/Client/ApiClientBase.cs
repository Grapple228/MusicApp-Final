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
using MusicClient.Exceptions;
using MusicClient.Helpers;
using MusicClient.Models.Tokens;
using RestSharp;

namespace MusicClient.Client;

public abstract class ApiClientBase : IApiClient
{
    public IJwtTokenModel Token { get; }
    public IRestClient RestClient { get; }
    public IIdentityApi IdentityApi { get; protected set; }
    public IAlbumsApi AlbumsApi { get; protected set;  }
    public IArtistsApi ArtistsApi { get; protected set;  }
    public IGenresApi GenresApi { get; protected set;  }
    public IPlaylistsApi PlaylistsApi { get; protected set;  }
    public ITracksApi TracksApi { get; protected set;  }
    public IUsersApi UsersApi { get; protected set;  }
    public IStreamingApi StreamingApi { get; protected set;  }
    public IImagesApi ImagesApi { get; protected set;  }

    private ApiClientBase(string gatewayPath)
    {
        if (!Uri.TryCreate($"{gatewayPath}/api", UriKind.Absolute, out var path))
            throw new InvalidUrlException();

        IdentityApi = this.GetDefaultIdentityApi();
        UsersApi = this.GetDefaultUsersApi();
        GenresApi = this.GetDefaultGenresApi();
        ArtistsApi = this.GetDefaultArtistsApi();
        AlbumsApi = this.GetDefaultAlbumsApi();
        PlaylistsApi = this.GetDefaultPlaylistsApi();
        TracksApi = this.GetDefaultTracksApi();
        StreamingApi = this.GetDefaultStreamingApi();
        ImagesApi = this.GetDefaultImagesApi();
        
        RestClient = new RestClient(path);
        Token = new DefaultJwtTokenModel("", "");
    }

    protected ApiClientBase(string gatewayPath, string accessToken, string refreshToken) : this(gatewayPath)
    {
        Token.ChangeToken(accessToken, refreshToken);
    }

    public virtual async Task<LoginDto> RefreshToken()
    {
        if (IdentityApi == null) throw new IdentityApiNullException();
        return await IdentityApi.RefreshToken(Token.RefreshToken);
    }

    public virtual void SetIdentityApi(IIdentityApi identityApi)
    {
        IdentityApi = identityApi;
    }
    public virtual void SetUsersApi(IUsersApi usersApi)
    {
        UsersApi = usersApi;
    }

    public virtual void SetGenresApi(IGenresApi genresApi)
    {
        GenresApi = genresApi;
    }
    
    public virtual void SetArtistsApi(IArtistsApi artistsApi)
    {
        ArtistsApi = artistsApi;
    }
    
    public virtual void SetAlbumsApi(IAlbumsApi albumsApi)
    {
        AlbumsApi = albumsApi;
    }
    
    public virtual void SetPlaylistsApi(IPlaylistsApi playlistsApi)
    {
        PlaylistsApi = playlistsApi;
    }
    
    public virtual void SetTracksApi(ITracksApi tracksApi)
    {
        TracksApi = tracksApi;
    }
    
    public virtual void SetStreamingApi(IStreamingApi streamingApi)
    {
        StreamingApi = streamingApi;
    }
    
    public virtual void SetImagesApi(IImagesApi imagesApi)
    {
        ImagesApi = imagesApi;
    }
}