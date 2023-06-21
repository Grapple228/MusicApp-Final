using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Playlist;

public class MediaPlaylistViewModel : LoadableViewModel, IModel, IOwnerable
{
    private bool _isSecondaryOwner;
    private bool _isUserOwner;

    private Models.Media.Playlists.Playlist _playlist = null!;

    public MediaPlaylistViewModel(Guid playlistId)
    {
        Id = playlistId;
        Task.Run(() => LoadInfo(playlistId));
        MediaEvents.PlaylistInfoChanged += OnPlaylistInfoChanged;
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;

        TrackEvents.TrackDeleted += TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested += TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved += TrackEventsOnTrackRemoved;
        TrackEvents.GenreAdded += TrackEventsOnGenreAdded;
        TrackEvents.GenreRemoved += TrackEventsOnGenreRemoved;

        ArtistEvents.ArtistRemoved += ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded += ArtistEventsOnArtistAdded;

        AlbumEvents.AlbumDeleted += AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoved += AlbumEventsOnAlbumRemoved;
        AlbumEvents.AddedToAlbum += AlbumEventsOnAddedToAlbum;

        PlaylistEvents.AddedToPlaylist += PlaylistEventsOnAddedToPlaylist;
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public static ContainerType ContainerType => ContainerType.Playlist;

    public Models.Media.Playlists.Playlist Playlist
    {
        get => _playlist;
        private set
        {
            if (Equals(value, _playlist)) return;
            _playlist = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Tracks));
            IsUserOwner = Playlist.IsUserOwner;
            IsSecondaryOwner = Playlist.IsSecondaryOwner;
            IsRemovable = Playlist.IsRemovable;
        }
    }

    public override string ModelName { get; protected set; } = "Playlist";

    public ObservableCollection<Models.Media.Genres.Genre> Genres { get; } = new();

    public ObservableCollection<TrackShort> Tracks => Playlist.Tracks;

    public Guid Id { get; init; }

    public bool IsUserOwner
    {
        get => _isUserOwner;
        set
        {
            if (value == _isUserOwner) return;
            _isUserOwner = value;
            OnPropertyChanged();
        }
    }

    public bool IsSecondaryOwner
    {
        get => _isSecondaryOwner;
        set
        {
            if (value == _isSecondaryOwner) return;
            _isSecondaryOwner = value;
            OnPropertyChanged();
        }
    }

    public bool IsRemovable { get; private set; }

    public override void Dispose()
    {
        MediaEvents.PlaylistInfoChanged -= OnPlaylistInfoChanged;
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;

        TrackEvents.TrackDeleted -= TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested -= TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved -= TrackEventsOnTrackRemoved;
        TrackEvents.GenreAdded -= TrackEventsOnGenreAdded;
        TrackEvents.GenreRemoved -= TrackEventsOnGenreRemoved;

        ArtistEvents.ArtistRemoved -= ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded -= ArtistEventsOnArtistAdded;

        AlbumEvents.AlbumDeleted -= AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoved -= AlbumEventsOnAlbumRemoved;
        AlbumEvents.AddedToAlbum -= AlbumEventsOnAddedToAlbum;

        PlaylistEvents.AddedToPlaylist -= PlaylistEventsOnAddedToPlaylist;
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    private void PlaylistEventsOnAddedToPlaylist(Guid playlistId, params Guid[] ids)
    {
        if (Id != playlistId) return;
        if (ids.Length == 1)
        {
            var track = Tracks.FirstOrDefault(x => x.Id == ids.First());
            if (track != null) return;
        }

        Task.Run(async () => await Reload());
    }

    private void TrackEventsOnGenreRemoved(Guid trackId, GenreDto genreDto)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        var genre = track.Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genre == null) return;
        ApplicationService.Invoke(() => track.Genres.Remove(genre));
        UpdateGenres();
    }

    private void TrackEventsOnGenreAdded(Guid trackId, GenreDto genreDto)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        var genre = track.Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genre != null) return;
        ApplicationService.Invoke(() => track.Genres.Add(genreDto));
        UpdateGenres();
    }

    private void TrackEventsOnTrackRemoved(Guid trackId, ContainerType containerType, Guid containerId)
    {
        if (ContainerType != containerType || containerId != Id) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
        UpdateGenres();
    }

    private void TrackEventsOnTrackRemoveRequested(Guid trackId, ContainerType containerType)
    {
        if (containerType != ContainerType) return;
        if (Tracks.FirstOrDefault(x => x.Id == trackId) == null) return;
        TrackEvents.Remove(trackId, containerType, Id);
    }

    ~MediaPlaylistViewModel()
    {
        Dispose();
    }

    private void ArtistEventsOnArtistRemoved(Guid artistId, ContainerType containerType, Guid containerId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == containerId);
        var artist = track?.Artists.FirstOrDefault(x => x.Id == artistId);
        if (artist == null) return;
        ApplicationService.Invoke(() => track?.Artists.Remove(artist));
    }

    private void AlbumEventsOnAlbumRemoved(Guid albumId, ContainerType containerType, Guid containerId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == containerId);
        var album = track?.Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => track?.Albums.Remove(album));
    }

    private void AlbumEventsOnAddedToAlbum(Guid albumId, Guid mediaId, ContainerType containerType)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == mediaId);
        if (track == null) return;
        if (track.Albums.FirstOrDefault(x => x.Id == albumId) != null) return;
        Task.Run(async () => await Reload());
    }

    private void ArtistEventsOnArtistAdded(Guid artistId, Guid mediaId, ContainerType containerType)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == mediaId);
        if (track == null) return;
        if (track.Artists.FirstOrDefault(x => x.Id == artistId) != null) return;
        Task.Run(async () => await Reload());
    }

    private void AlbumEventsOnAlbumDeleted(Guid albumId)
    {
        var tracks = Tracks.Where(x => x.Albums.Any(a => a.Id == albumId));
        foreach (var trackShort in tracks)
        {
            var album = trackShort.Albums.FirstOrDefault(x => x.Id == albumId);
            if (album == null) continue;
            ApplicationService.Invoke(() => trackShort.Albums.Remove(album));
        }
    }

    private void UpdateGenres()
    {
        var genres = Tracks.SelectMany(x => x.Genres).Distinct();
        ApplicationService.Invoke(() => Genres.Clear());
        foreach (var genreDto in genres)
            ApplicationService.Invoke(() => Genres.Add(new Models.Media.Genres.Genre(genreDto)));
    }

    private void TrackEventsOnTrackDeleted(Guid trackId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
        UpdateGenres();
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var playlist = await App.GetService().GetMediaPlaylist(mediaId);
        Playlist = new Models.Media.Playlists.Playlist(playlist);
        await MediaEvents.ChangePlaylistInfo(Playlist.Id, Playlist.IsLiked, Playlist.IsBlocked, false);
        UpdateGenres();
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    private void OnPlaylistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        if (Playlist.Id != id) return;
        Playlist.IsLiked = isLiked;
        Playlist.IsBlocked = isBlocked;
    }

    private void OnTrackInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == id);
        if (track == null) return;

        track.IsLiked = isLiked;
        track.IsBlocked = isBlocked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not MediaPlaylistViewModel vm) return false;
        return vm.Id == Id;
    }

    protected bool Equals(MediaPlaylistViewModel other)
    {
        return _playlist.Equals(other._playlist) && _isUserOwner == other._isUserOwner && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_playlist, _isUserOwner, Id);
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        if (CurrentTrack != null)
        {
            CurrentTrack.IsPlaying = false;
            CurrentTrack.IsCurrent = false;
            CurrentTrack = null;
        }
        Playlist.IsPlaying = false;
        Playlist.IsCurrent = false;
        
        if(ContainerType != containerType || Id != containerId) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if(track == null) return;
        if(CurrentTrack != null) CurrentTrack.IsPlaying = false;
        CurrentTrack = track;
        CurrentTrack.IsPlaying = Playlist.IsPlaying = isPlaying;
        CurrentTrack.IsCurrent = Playlist.IsCurrent = true;
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
    
}