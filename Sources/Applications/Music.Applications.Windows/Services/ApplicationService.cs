using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services.Navigation;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Authentication;
using Music.Applications.Windows.ViewModels.Default;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Applications.Windows.ViewModels.Navigation;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Media.Enums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Requests.Albums;
using Music.Shared.DTOs.Requests.Genres;
using Music.Shared.DTOs.Requests.Media;
using Music.Shared.DTOs.Requests.Playlists;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Common.DTOs;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Identity.Common.Requests;
using Music.Shared.Identity.Jwt;
using MusicClient.Api.Playlists;
using MusicClient.Client;
using MusicClient.Exceptions;
using MusicClient.Models.Tokens;
using Newtonsoft.Json;

namespace Music.Applications.Windows.Services;

public class ApplicationService
{
    private readonly IApiClient _client;

    public ApplicationService(IApiClient client)
    {
        _client = client;
        _client.Token.Changed += TokenOnChanged;
    }

    private static void TokenOnChanged(IToken token)
    {
        if (token is not IJwtTokenModel jwt) return;
        SaveToken(jwt.AccessToken, jwt.RefreshToken);
    }

    private static void SaveToken(string accessToken, string refreshToken)
    {
        var settings = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
        settings.SettingsModel.AccessToken = accessToken;
        settings.SettingsModel.RefreshToken = refreshToken;
        settings.SaveSettings();
    }

    private static void ChangeUser(LoginDto loginDto)
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        user.Change(loginDto);
    }

    private void ChangeToken(string? accessToken = null, string? refreshToken = null)
    {
        _client.Token.ChangeToken(accessToken ?? "", refreshToken ?? "");
    }

    private async Task Authenticate(LoginDto loginDto)
    {
        ChangeUser(loginDto);
        ChangeToken(loginDto.JwtAccessToken, loginDto.JwtRefreshToken);
        var defaultViewModel = App.ServiceProvider.GetRequiredService<DefaultViewModel>();
        await defaultViewModel.Load();
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();
        model.Navigation.SetCurrentView(defaultViewModel);
        var roomViewModel = App.ServiceProvider.GetRequiredService<RoomViewModel>();
        await roomViewModel.Connect();
    }

    public async Task<RegisterDto> Register(RegisterRequest request, bool isNeedDisplayMessage = false)
    {
        return await Execute(() => _client.IdentityApi.Register(request), isNeedDisplayMessage);
    }

    public async Task Authenticate(string username, string password, bool isDisplayMessage = false)
    {
        // TODO ADD DEVICE INFO
        var loginDto = await Execute(async () => await _client.IdentityApi.Authenticate(new LoginRequest(username,
            password,
            new DeviceInfo { DeviceHash = "hash1" })), isLoginPage: true, isNeedToDisplayMessage: isDisplayMessage);
        await Authenticate(loginDto);
    }

    public async Task Authenticate(string? refreshToken, bool isDisplayMessage = false)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            App.ServiceProvider.GetRequiredService<GlobalNavigationService>()
                .NavigateTo<AuthenticationViewModel>();
            return;
        }

        try
        {
            var loginDto = await Execute(async () => await _client.IdentityApi.RefreshToken(refreshToken),
                isDisplayMessage);
            await Authenticate(loginDto);
        }
        catch (ServerUnavailableException)
        {
            App.ServiceProvider.GetRequiredService<GlobalNavigationService>()
                .NavigateTo<AuthenticationViewModel>();
        }
    }

    public async Task<MemoryStream> GetStream(Guid trackId, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.StreamingApi.GetStream(trackId),
            isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaTrackDto>> GetContainerTracks(Guid containerId,
        ContainerType containerType, bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                return await Execute(async () => await _client.PlaylistsApi.GetTracks(containerId), isDisplayMessage);
            case ContainerType.Album:
                return await Execute(async () => await _client.AlbumsApi.GetTracks(containerId), isDisplayMessage);
            case ContainerType.Artist:
                return await Execute(async () => await _client.ArtistsApi.GetTracks(containerId), isDisplayMessage);
            default:
                throw new ArgumentOutOfRangeException(nameof(containerType), containerType, null);
        }
    }


    public async Task DeleteUser(Guid userId, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.IdentityApi.DeleteUser(userId), isDisplayMessage);
    }

    public async Task DeleteUser(bool isDisplayMessage = true)
    {
        await Execute(async () =>
        {
            await _client.IdentityApi.DeleteUser();
        }, isDisplayMessage);
    }

    public async Task<IdentityUserDto> GetProfile(bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.IdentityApi.GetProfile(),
            isNeedToDisplayMessage: isDisplayMessage);
    }
    
    public async Task AddRole(Guid userId, Roles role, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.IdentityApi.AddRole(userId, role),
            isDisplayMessage);
    }

    public async Task RemoveRole(Guid userId, Roles role, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.IdentityApi.RemoveRole(userId, role),
            isDisplayMessage);
    }

    public async Task ChangeAlbumInfo(Guid albumId, bool isLiked, bool isBlocked, bool isDisplayMessage = true)
    {
        await Execute(async () =>
            await _client.AlbumsApi.ChangeMedia(albumId, new MediaCreateRequest(isLiked, isBlocked)), isDisplayMessage);
    }

    public async Task ChangeTrackInfo(Guid trackId, bool isLiked, bool isBlocked, bool isDisplayMessage = true)
    {
        await Execute(async () =>
            await _client.TracksApi.ChangeMedia(trackId, new MediaCreateRequest(isLiked, isBlocked)), isDisplayMessage);
    }

    public async Task ChangeArtistInfo(Guid artistId, bool isLiked, bool isBlocked, bool isDisplayMessage = true)
    {
        await Execute(async () =>
                await _client.ArtistsApi.ChangeMedia(artistId, new MediaCreateRequest(isLiked, isBlocked)),
            isDisplayMessage);
    }

    public async Task ChangePlaylistInfo(Guid playlistId, bool isLiked, bool isBlocked, bool isDisplayMessage = true)
    {
        await Execute(async () =>
                await _client.PlaylistsApi.ChangeMedia(playlistId, new MediaCreateRequest(isLiked, isBlocked)),
            isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<IdentityUserDto>> GetIdentityUsers(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.IdentityApi.GetProfiles(), isDisplayMessage);
    }

    public async Task<UserDto> GetUser(Guid userId, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.UsersApi.Get(userId), isNeedToDisplayMessage: isDisplayMessage);
    }

    public async Task<IdentityUserDto> GetIdentityUser(Guid userId, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.IdentityApi.GetProfile(userId), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<GenreDto>> GetGenres(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.GenresApi.GetAll(), isDisplayMessage);
    }

    public async Task<GenreDto> GetGenre(Guid genreId, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.GenresApi.Get(genreId), isDisplayMessage);
    }

    public async Task DeleteGenre(Guid genreId, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.GenresApi.Delete(genreId), isDisplayMessage);
    }

    public async Task<GenreDto> CreateGenre(GenreCreateRequest request, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.GenresApi.Create(request), isDisplayMessage);
    }

    public async Task<GenreDto> UpdateGenre(Guid genreId, GenreUpdateRequest request, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.GenresApi.Update(genreId, request),
            isDisplayMessage);
    }

    public async Task AddGenre(Guid trackId, Guid genreId, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.TracksApi.AddGenre(trackId, genreId), isDisplayMessage);
    }

    public async Task RemoveGenre(Guid trackId, Guid genreId, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.TracksApi.RemoveGenre(trackId, genreId), isDisplayMessage);
    }

    public async Task<TrackDto> GetTrack(Guid trackId, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.TracksApi.Get(trackId), isDisplayMessage);
    }

    public async Task<MediaAlbumDto> GetMediaAlbum(Guid albumId, bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.AlbumsApi.GetMedia(albumId), isDisplayMessage);
    }

    public async Task<MediaArtistDto> GetMediaArtist(Guid artistId, bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.ArtistsApi.GetMedia(artistId), isDisplayMessage);
    }

    public async Task<MediaTrackDto> GetMediaTrack(Guid trackId, bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.TracksApi.GetMedia(trackId), isDisplayMessage);
    }

    public async Task<MediaPlaylistDto> GetMediaPlaylist(Guid playlistId, bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.PlaylistsApi.GetMedia(playlistId), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaAlbumDto>> GetMediaAlbums(MediaFilterEnum filter = MediaFilterEnum.All,
        bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.AlbumsApi.GetAllMedia(filter), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaArtistDto>> GetMediaArtists(MediaFilterEnum filter = MediaFilterEnum.All,
        bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.ArtistsApi.GetAllMedia(filter), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaTrackDto>> GetMediaTracks(MediaFilterEnum filter = MediaFilterEnum.All,
        bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.TracksApi.GetAllMedia(filter), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaPlaylistDto>> GetMediaPlaylists(
        MediaFilterEnum filter = MediaFilterEnum.All, bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.PlaylistsApi.GetAllMedia(filter), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaAlbumDto>> GetUserMediaAlbums(bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.AlbumsApi.GetAllUserMedia(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaArtistDto>> GetUserMediaArtists(bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.ArtistsApi.GetAllUserMedia(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaTrackDto>> GetUserMediaTracks(bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.TracksApi.GetAllUserMedia(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<MediaPlaylistDto>> GetUserMediaPlaylists(bool isDisplayMessage = false)
    {
        return await Execute(async () => await _client.PlaylistsApi.GetAllUserMedia(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<PlaylistDto>> GetUserPlaylists(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.PlaylistsApi.GetAllUser(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<TrackDto>> GetUserTracks(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.TracksApi.GetAllUser(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<AlbumDto>> GetUserAlbums(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.AlbumsApi.GetAllUser(), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<ArtistDto>> GetArtists(long count = 30, string? searchQuery = null, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.ArtistsApi.GetAll(count, searchQuery), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<TrackDto>> GetTracks(long count = 30, string? searchQuery = null, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.TracksApi.GetAll(count, searchQuery), isDisplayMessage);
    }

    public async Task<IReadOnlyCollection<AlbumDto>> GetAlbums(long count = 30, string? searchQuery = null, bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.AlbumsApi.GetAll(count, searchQuery), isDisplayMessage);
    }

    public async Task CreatePlaylist(PlaylistCreateRequest request, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () =>
            {
                var playlist = await _client.PlaylistsApi.Create(request);
                PlaylistEvents.Create(new UserPlaylist(playlist));
                return playlist;
            }, isDisplayMessage);
        }
        catch
        {
            // ignored
        }
    }

    public async Task CreateTrack(CreateTrackViewModel createTrackViewModel, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () =>
            {
                var trackDto = await _client.TracksApi.Create(
                    createTrackViewModel.Title,
                    createTrackViewModel.Genres.Select(x => x.Id).ToArray(),
                    DateOnly.FromDateTime(createTrackViewModel.PublicationDate!.Value),
                    createTrackViewModel.Filename,
                    createTrackViewModel.IsArtist ? null : createTrackViewModel.ArtistId);

                TrackEvents.Create(trackDto);
                return trackDto;
            }, isDisplayMessage);
        }
        catch
        {
            // ignored
        }
    }

    public async Task CreateAlbum(AlbumCreateRequest request, Guid? artistId = null, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () =>
            {
                var albumDto = await _client.AlbumsApi.Create(request, artistId);

                AlbumEvents.Create(albumDto);
                return albumDto;
            }, isDisplayMessage);
        }
        catch
        {
            // ignored
        }
    }

    public async Task DeletePlaylist(Guid playlistId, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () => await _client.PlaylistsApi.Delete(playlistId),
                isDisplayMessage);
            PlaylistEvents.Delete(playlistId);

            var defaultViewModel = App.ServiceProvider.GetRequiredService<DefaultViewModel>();

            if (defaultViewModel.Navigation.CurrentView is MediaPlaylistViewModel model
                && model.Playlist.Id == playlistId)
                defaultViewModel.Navigation.NavigateTo<HomeViewModel>();
        }
        catch
        {
            // ignored
        }
    }

    public async Task DeleteAlbum(Guid albumId, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () => await _client.AlbumsApi.Delete(albumId), isDisplayMessage);
            AlbumEvents.Delete(albumId);

            var defaultViewModel = App.ServiceProvider.GetRequiredService<DefaultViewModel>();

            if (defaultViewModel.Navigation.CurrentView is MediaAlbumViewModel model
                && model.Album.Id == albumId)
                defaultViewModel.Navigation.NavigateTo<HomeViewModel>();
        }
        catch
        {
            // ignored
        }
    }

    public async Task DeleteTrack(Guid trackId, bool isDisplayMessage = true)
    {
        try
        {
            await Execute(async () => await _client.TracksApi.Delete(trackId), isDisplayMessage);
            TrackEvents.Delete(trackId);

            var defaultViewModel = App.ServiceProvider.GetRequiredService<DefaultViewModel>();

            if (defaultViewModel.Navigation.CurrentView is MediaTrackViewModel model
                && model.Track.Id == trackId)
                defaultViewModel.Navigation.NavigateTo<HomeViewModel>();
        }
        catch
        {
            // ignored
        }
    }

    public async Task RemoveTrack(Guid containerId, Guid trackId, ContainerType containerType,
        bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                await Execute(async () => await _client.PlaylistsApi.RemoveTrack(containerId, trackId),
                    isDisplayMessage);
                break;
            case ContainerType.Album:
                await Execute(async () => await _client.AlbumsApi.RemoveTrack(containerId, trackId),
                    isDisplayMessage);
                break;
            case ContainerType.Artist:
                await Execute(async () => await _client.TracksApi.RemoveArtist(trackId, containerId),
                    isDisplayMessage);
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(containerType), containerType, null);
        }
    }

    public async Task RemoveAlbum(Guid containerId, Guid albumId, ContainerType containerType,
        bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.Album:
                break;
            case ContainerType.Artist:
                await Execute(async () => await _client.AlbumsApi.RemoveArtist(albumId, containerId),
                    isDisplayMessage);
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                await Execute(async () => await _client.AlbumsApi.RemoveTrack(albumId, containerId),
                    isDisplayMessage);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(containerType), containerType, null);
        }
    }

    public async Task RemoveArtist(Guid containerId, Guid artistId, ContainerType containerType,
        bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.Album:
                await Execute(async () => await _client.AlbumsApi.RemoveArtist(containerId, artistId),
                    isDisplayMessage);
                break;
            case ContainerType.Artist:
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                await Execute(async () => await _client.TracksApi.RemoveArtist(containerId, artistId),
                    isDisplayMessage);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(containerType), containerType, null);
        }
    }

    public async Task AddTracksToPlaylist(Guid playlistId, Guid mediaId, AddToPlaylist addToPlaylist,
        bool isDisplayMessage = true)
    {
        await Execute(async () =>
            {
                await _client.PlaylistsApi.AddTracks(playlistId, mediaId, addToPlaylist);
                PlaylistEvents.AddToPlaylist(playlistId);
            },
            isDisplayMessage);
    }

    public async Task AddTrackToPlaylist(Guid playlistId, Guid trackId, bool isDisplayMessage = true)
    {
        await Execute(async () =>
            {
                await _client.PlaylistsApi.AddTrack(playlistId, trackId);
                PlaylistEvents.AddToPlaylist(playlistId, trackId);
            },
            isDisplayMessage);
    }

    public async Task ChangePassword(ChangePasswordRequest request, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.IdentityApi.ChangePassword(request), isNeedToDisplayMessage: isDisplayMessage);
    }
    
    public async Task ChangeUsername(string username, bool isDisplayMessage = true)
    {
        await Execute(async () => await _client.IdentityApi.ChangeUsername(username), isNeedToDisplayMessage: isDisplayMessage);
    }
    
    public async Task<RoomDto> CreateRoom(bool isDisplayMessage = true)
    {
        return await Execute(async () => await _client.StreamingApi.CreateRoom(),
            isNeedToDisplayMessage: isDisplayMessage);
    }

    public async Task AddToAlbum(Guid albumId, Guid mediaId, ContainerType containerType, bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.Album:
                break;
            case ContainerType.Artist:
                await Execute(() => _client.AlbumsApi.AddArtist(albumId, mediaId), isDisplayMessage);
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                await Execute(() => _client.AlbumsApi.AddTrack(albumId, mediaId), isDisplayMessage);
                break;
        }
    }

    public async Task AddArtist(Guid artistId, Guid mediaId, ContainerType containerType, bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.Album:
                await Execute(() => _client.AlbumsApi.AddArtist(mediaId, artistId), isDisplayMessage);
                break;
            case ContainerType.Artist:
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                await Execute(() => _client.TracksApi.AddArtist(mediaId, artistId), isDisplayMessage);
                break;
        }
    }

    public async Task AddTrack(Guid trackId, Guid mediaId, ContainerType containerType, bool isDisplayMessage = true)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.Album:
                await Execute(() => _client.AlbumsApi.AddTrack(mediaId, trackId), isDisplayMessage);
                break;
            case ContainerType.Artist:
                await Execute(() => _client.TracksApi.AddArtist(trackId, mediaId), isDisplayMessage);
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                break;
        }
    }

    public async Task ChangeAudio(Guid trackId, bool isDisplayMessage = true)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Audio (*.mp3) | *.mp3",
            Title = "Change Audio"
        };
        var result = ofd.ShowDialog();
        if (!result.HasValue || !result.Value) return;

        var filename = ofd.FileName;

        await Execute(async () =>
            {
                var duration = await _client.TracksApi.ChangeFile(trackId, filename);
                TrackEvents.ChangeDuration(trackId, duration);
                return duration;
            },
            isDisplayMessage);
    }

    public async Task ChangeImage(Guid id, ChangeImage changeImage, bool isDisplayMessage = true)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Image (*.png; *.jpg) | *.png; *.jpg; *.jpeg",
            Title = "Load image"
        };
        var result = ofd.ShowDialog();
        if (!result.HasValue || !result.Value) return;

        var filename = ofd.FileName;

        switch (changeImage)
        {
            case Events.ChangeImage.Albums:
                await Execute(async () =>
                {
                    await _client.ImagesApi.ChangeAlbum(id, filename);
                    ImageEvents.Update(id, changeImage);
                }, isDisplayMessage);
                break;
            case Events.ChangeImage.Artists:
                await Execute(async () =>
                {
                    await _client.ImagesApi.ChangeArtist(id, filename);
                    ImageEvents.Update(id, changeImage);
                }, isDisplayMessage);
                break;
            case Events.ChangeImage.Tracks:
                await Execute(async () =>
                {
                    await _client.ImagesApi.ChangeTrack(id, filename);
                    ImageEvents.Update(id, changeImage);
                }, isDisplayMessage);
                break;
            case Events.ChangeImage.Playlists:
                await Execute(async () =>
                {
                    await _client.ImagesApi.ChangePlaylist(id, filename);
                    ImageEvents.Update(id, changeImage);
                }, isDisplayMessage);
                break;
            case Events.ChangeImage.Users:
                await Execute(async () =>
                {
                    await _client.ImagesApi.ChangeUser(id, filename);
                    ImageEvents.Update(id, changeImage);
                }, isDisplayMessage);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(changeImage), changeImage, null);
        }
    }

    public static void NavigateTo<TViewModelBase>(TViewModelBase viewModelBase)
        where TViewModelBase : ViewModelBase
    {
        var nav = App.ServiceProvider.GetRequiredService<ApplicationNavigationService>();
        nav.SetCurrentView(viewModelBase);
    }

    public static void NavigateTo<TViewModelBase>()
        where TViewModelBase : ViewModelBase
    {
        var nav = App.ServiceProvider.GetRequiredService<ApplicationNavigationService>();
        nav.NavigateTo<TViewModelBase>();
    }

    public void SignOut(string message = "")
    {
        ChangeToken();
        GlobalEvents.SignOut(message);
        PlayerEvents.RequestClear();
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        var auth = new AuthenticationViewModel(user.Username);
        auth.ChangeErrorMessage(message);
        var navigation = App.ServiceProvider.GetRequiredService<GlobalNavigationService>();
        navigation.SetCurrentView(auth);
    }

    private async Task<T> Execute<T>(Func<Task<T>> function, bool isNeedToDisplayMessage = false,
        bool isLoginPage = false)
    {
        try
        {
            return await function();
        }
        catch (Exception e)
        {
            ProcessException(e, isNeedToDisplayMessage, isLoginPage);
            throw;
        }
    }

    private async Task Execute(Func<Task> function, bool isNeedToDisplayMessage = false, bool isLoginPage = false)
    {
        try
        {
            await function();
        }
        catch (Exception e)
        {
            ProcessException(e, isNeedToDisplayMessage, isLoginPage);
            throw;
        }
    }

    public static bool IsSecondaryOwner(IEnumerable<Guid> owners)
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return owners.Contains(user.Id);
    }

    public static bool IsSame(Guid? userId)
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return user.Id == userId;
    }

    public static Guid CurrentUserId()
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return user.Id;
    }

    public string GetToken()
    {
        return _client.Token.AccessToken;
    }

    public static bool IsUserOwner(Guid? userId)
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return user.Id == userId;
    }

    public static bool IsUserAdmin()
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return user.Roles.Contains(Roles.Admin);
    }

    public static bool IsUserArtist()
    {
        var user = App.ServiceProvider.GetRequiredService<CurrentUser>();
        return user.Roles.Contains(Roles.Artist);
    }

    private void ProcessException(Exception ex, bool isNeedToDisplayMessage, bool isLoginPage = false)
    {
        const string tokenExpiredMessage = "Token is expired. Please, login again";

        switch (ex)
        {
            case NullTokenException:
                // Выйти из аккаунта
                SignOut(tokenExpiredMessage);
                if (isNeedToDisplayMessage) DisplayMessage(tokenExpiredMessage);
                throw ex;
            case InvalidRefreshTokenException:
                // Выйти из аккаунта
                SignOut(tokenExpiredMessage);
                if (isNeedToDisplayMessage) DisplayMessage(tokenExpiredMessage);
                throw ex;
            case InvalidAuthorizationDataException when isLoginPage:
                // Если на странице авторизации, то просто бросить исключение
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            case InvalidAuthorizationDataException:
                // Выйти из аккаунта, а после бросить исключение
                SignOut(tokenExpiredMessage);
                if (isNeedToDisplayMessage) DisplayMessage(tokenExpiredMessage);
                throw ex;
            case ServerUnavailableException:
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            case Status500Exception:
            case JsonReaderException:
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw new ServerUnavailableException();
            case Status400Exception:
                // Вывести сообщение о некорректном запросе
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            case Status403Exception:
                // Вывести сообщение об отсутствии прав
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            case Status404Exception:
                // Вывести сообщение о ненахождении модели
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            case Status409Exception:
                // Вывести сообщение о неправильном запросе
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw ex;
            default:
                // Вывести сообщение о непредвиденной ошибке   
                if (isNeedToDisplayMessage) DisplayMessage(ex.Message);
                throw new Exception("Unhandled exception");
        }

        void DisplayMessage(string message)
        {
            NotificationEvents.DisplayNotification(message, "Error", NotificationType.Error);
        }
    }

    public static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scv = (ScrollViewer)sender;
        scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / Constants.ScrollSizeDividing);
        e.Handled = true;
    }

    public static void Invoke(Action a)
    {
        Application.Current.Dispatcher.Invoke(a);
    }
}