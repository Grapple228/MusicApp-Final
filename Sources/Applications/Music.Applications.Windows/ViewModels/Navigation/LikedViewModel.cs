using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Models.Media.Playlists;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Media.Enums;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class LikedViewModel : LoadableViewModel, IDisplaying
{
    private bool _isFirstLoad = true;

    public LikedViewModel()
    {
        Task.Run(() => LoadInfo(Guid.Empty));
        IsFirstLoad = false;
        MediaEvents.PlaylistInfoChanged += OnPlaylistInfoChanged;
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged += OnArtistInfoChanged;
        MediaEvents.AlbumInfoChanged += OnAlbumInfoChanged;

        AlbumEvents.AlbumDeleted += AlbumEventsOnAlbumDeleted;
        PlaylistEvents.PlaylistDeleted += PlaylistEventsOnPlaylistDeleted;
        TrackEvents.TrackDeleted += TrackEventsOnTrackDeleted;
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public static ContainerType ContainerType => ContainerType.Liked;

    public bool IsFirstLoad
    {
        get => _isFirstLoad;
        set
        {
            if (value == _isFirstLoad) return;
            _isFirstLoad = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Track> Tracks { get; } = new();
    public ObservableCollection<Album> Albums { get; } = new();
    public ObservableCollection<Artist> Artists { get; } = new();
    public ObservableCollection<Playlist> Playlists { get; } = new();

    public override string ModelName { get; protected set; } = "Liked";

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.LikedViewModel;
        set
        {
            if (DisplayingMedia.LikedViewModel.Equals(value)) return;
            DisplayingMedia.LikedViewModel = value;
            OnPropertyChanged();
            Task.Run(async () => await Reload());
            IsFirstLoad = false;
        }
    }

    private void TrackEventsOnTrackDeleted(Guid trackId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
    }

    private void PlaylistEventsOnPlaylistDeleted(Guid playlistId)
    {
        var playlist = Playlists.FirstOrDefault(x => x.Id == playlistId);
        if (playlist == null) return;
        ApplicationService.Invoke(() => Playlists.Remove(playlist));
    }

    private void AlbumEventsOnAlbumDeleted(Guid albumId)
    {
        var album = Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => Albums.Remove(album));
    }

    public override void Dispose()
    {
        MediaEvents.PlaylistInfoChanged -= OnPlaylistInfoChanged;
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged -= OnArtistInfoChanged;
        MediaEvents.AlbumInfoChanged -= OnAlbumInfoChanged;

        AlbumEvents.AlbumDeleted -= AlbumEventsOnAlbumDeleted;
        PlaylistEvents.PlaylistDeleted -= AlbumEventsOnAlbumDeleted;
        TrackEvents.TrackDeleted -= TrackEventsOnTrackDeleted;
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;

        base.Dispose();
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        switch (CurrentDisplaying)
        {
            case CurrentDisplaying.Artists:
                if (CurrentArtist != null)
                {
                    CurrentArtist.IsPlaying = false;
                    CurrentArtist.IsCurrent = false;
                    CurrentArtist = null;
                }
                if(containerType != ContainerType.Artist) return;
                var artist = Artists.FirstOrDefault(x => x.Id == containerId);
                if(artist == null) return;
                CurrentArtist = artist;
                CurrentArtist.IsCurrent = true;
                CurrentArtist.IsPlaying = isPlaying;
                break;
            case CurrentDisplaying.Tracks:
                if (CurrentTrack != null)
                {
                    CurrentTrack.IsPlaying = false;
                    CurrentTrack.IsCurrent = false;
                    CurrentTrack = null;
                }
                
                var track = Tracks.FirstOrDefault(x => x.Id == trackId);
                if(track == null) return;
                CurrentTrack = track;
                CurrentTrack.IsCurrent = true;
                CurrentTrack.IsPlaying = isPlaying;
                break;
            case CurrentDisplaying.Albums:
                if (CurrentAlbum != null)
                {
                    CurrentAlbum.IsPlaying = false;
                    CurrentAlbum.IsCurrent = false;
                    CurrentAlbum = null;
                }
                if(containerType != ContainerType.Album) return;
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if(album == null) return;
                CurrentAlbum = album;
                CurrentAlbum.IsCurrent = true;
                CurrentAlbum.IsPlaying = isPlaying;
                break;
            case CurrentDisplaying.Playlists:
                if (CurrentPlaylist != null)
                {
                    CurrentPlaylist.IsPlaying = false;
                    CurrentPlaylist.IsCurrent = false;
                    CurrentPlaylist = null;
                }
                if(containerType != ContainerType.Playlist) return;
                var playlist = Playlists.FirstOrDefault(x => x.Id == containerId);
                if(playlist == null) return;
                CurrentPlaylist = playlist;
                CurrentPlaylist.IsCurrent = true;
                CurrentPlaylist.IsPlaying = isPlaying;
                break;
        }
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        try
        {
            switch (CurrentDisplaying)
            {
                case CurrentDisplaying.Artists:
                    await LoadArtists();
                    break;
                case CurrentDisplaying.Tracks:
                    await LoadTracks();
                    break;
                case CurrentDisplaying.Albums:
                    await LoadAlbums();
                    break;
                case CurrentDisplaying.Playlists:
                    await LoadPlaylists();
                    break;
            }
            
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
        }
        catch
        {
            // ignored
        }

        async Task LoadArtists()
        {
            var artists = await App.GetService().GetMediaArtists(MediaFilterEnum.Liked);
            ApplicationService.Invoke(() => Artists.Clear());
            foreach (var artist in artists) ApplicationService.Invoke(() => Artists.Add(new Artist(artist)));
            
        }

        async Task LoadAlbums()
        {
            var albums = await App.GetService().GetMediaAlbums(MediaFilterEnum.Liked);
            ApplicationService.Invoke(() => Albums.Clear());
            foreach (var album in albums) ApplicationService.Invoke(() => Albums.Add(new Album(album)));
        }

        async Task LoadTracks()
        {
            var tracks = await App.GetService().GetMediaTracks(MediaFilterEnum.Liked);
            ApplicationService.Invoke(() => Tracks.Clear());
            foreach (var track in tracks) ApplicationService.Invoke(() => Tracks.Add(new Track(track)));
        }

        async Task LoadPlaylists()
        {
            var playlists = await App.GetService().GetMediaPlaylists(MediaFilterEnum.Liked);
            ApplicationService.Invoke(() => Playlists.Clear());
            foreach (var playlist in playlists) ApplicationService.Invoke(() => Playlists.Add(new Playlist(playlist)));
        }
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    private void OnTrackInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == id);
        if (track == null) return;

        track.IsLiked = isLiked;
        track.IsBlocked = isBlocked;
    }

    private void OnAlbumInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var album = Albums.FirstOrDefault(x => x.Id == id);
        if (album == null) return;

        album.IsLiked = isLiked;
        album.IsBlocked = isBlocked;
    }

    private void OnArtistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var artist = Artists.FirstOrDefault(x => x.Id == id);
        if (artist == null) return;

        artist.IsLiked = isLiked;
        artist.IsBlocked = isBlocked;
    }

    private void OnPlaylistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var playlist = Playlists.FirstOrDefault(x => x.Id == id);
        if (playlist == null) return;

        playlist.IsLiked = isLiked;
        playlist.IsBlocked = isBlocked;
    }

    public IPlaylist? CurrentPlaylist
    {
        get => _currentPlaylist;
        set
        {
            if (Equals(value, _currentPlaylist)) return;
            _currentPlaylist = value;
            OnPropertyChanged();
        }
    }

    private IAlbum? _currentAlbum;
    
    public IAlbum? CurrentAlbum
    {
        get => _currentAlbum;
        set
        {
            if (Equals(value, _currentAlbum)) return;
            _currentAlbum = value;
            OnPropertyChanged();
        }
    }
    
    private ITrack? _currentTrack;
    
    public ITrack? CurrentTrack
    {
        get => _currentTrack;
        set
        {
            if (Equals(value, _currentTrack)) return;
            _currentTrack = value;
            OnPropertyChanged();
        }
    }
    
    private IArtist? _currentArtist;
    private IPlaylist? _currentPlaylist;

    public IArtist? CurrentArtist
    {
        get => _currentArtist;
        set
        {
            if (Equals(value, _currentArtist)) return;
            _currentArtist = value;
            OnPropertyChanged();
        }
    }
}