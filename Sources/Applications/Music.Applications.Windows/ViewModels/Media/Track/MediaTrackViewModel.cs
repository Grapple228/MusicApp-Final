using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Track;

public class MediaTrackViewModel : LoadableViewModel, IModel, IDisplaying, IOwnerable
{
    private bool _isSecondaryOwner;

    private bool _isUserOwner;

    private Models.Media.Tracks.Track _track = null!;

    public MediaTrackViewModel(Guid trackId)
    {
        Id = trackId;
        Task.Run(() => LoadInfo(trackId));
        MediaEvents.AlbumInfoChanged += OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged += OnArtistInfoChanged;

        AlbumEvents.AlbumRemoveRequested += AlbumEventsOnAlbumRemoveRequested;
        AlbumEvents.AlbumDeleted += AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoved += AlbumEventsOnAlbumRemoved;
        AlbumEvents.AddedToAlbum += AlbumEventsOnAddedToAlbum;

        TrackEvents.TrackRemoved += TrackEventsOnTrackRemoved;
        TrackEvents.GenreRemoved += TrackEventsOnGenreRemoved;
        TrackEvents.GenreAdded += TrackEventsOnGenreAdded;

        ArtistEvents.ArtistRemoveRequested += ArtistEventsOnArtistRemoveRequested;
        ArtistEvents.ArtistRemoved += ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded += ArtistEventsOnArtistAdded;
        
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public static ContainerType ContainerType => ContainerType.Track;

    public ObservableCollection<ArtistShort> Artists => Track.Artists;
    public ObservableCollection<AlbumShort> Albums => Track.Albums;
    public ObservableCollection<Models.Media.Genres.Genre> Genres { get; } = new();

    public Models.Media.Tracks.Track Track
    {
        get => _track;
        set
        {
            if (Equals(value, _track)) return;
            _track = value;
            OnPropertyChanged(nameof(Artists));
            OnPropertyChanged(nameof(Albums));
            OnPropertyChanged(nameof(Genres));
            OnPropertyChanged();
            IsUserOwner = Track.IsUserOwner;
            IsSecondaryOwner = Track.IsSecondaryOwner;
            IsRemovable = Track.IsRemovable;
            UpdateGenres();
        }
    }

    public override string ModelName { get; protected set; } = "Track";

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.TrackViewModel;
        set
        {
            DisplayingMedia.TrackViewModel = value;
            OnPropertyChanged();
        }
    }

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

    private void TrackEventsOnGenreAdded(Guid trackId, GenreDto genreDto)
    {
        if (Id != trackId) return;
        if (Genres.FirstOrDefault(x => x.Id == genreDto.Id) != null) return;
        ApplicationService.Invoke(() => Genres.Add(new Models.Media.Genres.Genre(genreDto)));
    }

    private void TrackEventsOnGenreRemoved(Guid trackId, GenreDto genreDto)
    {
        if (Id != trackId) return;
        var genre = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genre == null) return;
        ApplicationService.Invoke(() => Genres.Remove(genre));
    }

    private void AlbumEventsOnAddedToAlbum(Guid albumId, Guid mediaId, ContainerType containerType)
    {
        if (mediaId != Id || ContainerType != containerType) return;
        if (Albums.FirstOrDefault(x => x.Id == albumId) != null) return;
        Task.Run(async () => await Reload());
    }

    private void ArtistEventsOnArtistRemoved(Guid artistId, ContainerType containerType, Guid containerId)
    {
        if (containerType != ContainerType) return;
        if (containerId != Id) return;
        var artist = Artists.FirstOrDefault(x => x.Id == artistId);
        if (artist == null) return;
        ApplicationService.Invoke(() => Artists.Remove(artist));
    }

    private void ArtistEventsOnArtistRemoveRequested(Guid artistId, ContainerType containerType)
    {
        if (containerType != ContainerType) return;
        if (Artists.FirstOrDefault(x => x.Id == artistId) == null) return;
        ArtistEvents.Remove(artistId, containerType, Id);
    }

    private void AlbumEventsOnAlbumRemoved(Guid albumId, ContainerType containerType, Guid containerId)
    {
        if (containerType != ContainerType) return;
        if (containerId != Id) return;
        var album = Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => Albums.Remove(album));
    }

    private void AlbumEventsOnAlbumRemoveRequested(Guid albumId, ContainerType containerType)
    {
        if (containerType != ContainerType) return;
        if (Albums.FirstOrDefault(x => x.Id == albumId) == null) return;
        AlbumEvents.Remove(albumId, containerType, Id);
    }

    private void TrackEventsOnTrackRemoved(Guid trackId, ContainerType containerType, Guid containerId)
    {
        switch (containerType)
        {
            case ContainerType.Playlist:
                break;
            case ContainerType.User:
                break;
            case ContainerType.Track:
                break;
            case ContainerType.Album:
                if (trackId != Id) return;
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if (album == null) return;
                ApplicationService.Invoke(() => Albums.Remove(album));
                break;
            case ContainerType.Artist:
                if (trackId != Id) return;
                var artist = Artists.FirstOrDefault(x => x.Id == containerId);
                if (artist == null) return;
                ApplicationService.Invoke(() => Artists.Remove(artist));
                break;
        }
    }

    private void AlbumEventsOnAlbumDeleted(Guid albumId)
    {
        var album = Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => Albums.Remove(album));
    }

    private void UpdateGenres()
    {
        ApplicationService.Invoke(() => Genres.Clear());
        foreach (var trackGenre in _track.Genres)
            ApplicationService.Invoke(() => Genres.Add(new Models.Media.Genres.Genre(trackGenre)));
    }

    public override void Dispose()
    {
        MediaEvents.AlbumInfoChanged -= OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged -= OnArtistInfoChanged;

        AlbumEvents.AlbumDeleted -= AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoveRequested -= AlbumEventsOnAlbumRemoveRequested;
        AlbumEvents.AddedToAlbum -= AlbumEventsOnAddedToAlbum;

        AlbumEvents.AlbumRemoved -= AlbumEventsOnAlbumRemoved;
        TrackEvents.TrackRemoved -= TrackEventsOnTrackRemoved;
        TrackEvents.GenreRemoved -= TrackEventsOnGenreRemoved;
        TrackEvents.GenreAdded -= TrackEventsOnGenreAdded;

        ArtistEvents.ArtistRemoveRequested -= ArtistEventsOnArtistRemoveRequested;
        ArtistEvents.ArtistRemoved -= ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded -= ArtistEventsOnArtistAdded;

        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;
        
        GC.SuppressFinalize(this);
    }

    private void ArtistEventsOnArtistAdded(Guid artistId, Guid mediaId, ContainerType containerType)
    {
        switch (containerType)
        {
            case ContainerType.Album:
                var album = Albums.FirstOrDefault(x => x.Id == mediaId);
                if (album == null) return;
                if (album.Artists.Any(x => x.Id == artistId)) return;
                break;
            case ContainerType.Track:
                if (mediaId != Id) return;
                if (Artists.FirstOrDefault(x => x.Id == artistId) != null) return;
                break;
            default:
                return;
        }

        Task.Run(async () => await Reload());
    }

    ~MediaTrackViewModel()
    {
        Dispose();
    }

    protected override async Task LoadMedia(Guid trackId)
    {
        var track = await App.GetService().GetMediaTrack(trackId);
        Track = new Models.Media.Tracks.Track(track);
        await MediaEvents.ChangeTrackInfo(Track.Id, Track.IsLiked, Track.IsBlocked, false);
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    private void OnArtistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var artist = Artists.FirstOrDefault(x => x.Id == id);
        if (artist == null) return;

        artist.IsLiked = isLiked;
        artist.IsBlocked = isBlocked;
    }

    private void OnAlbumInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var album = Albums.FirstOrDefault(x => x.Id == id);
        if (album == null) return;

        album.IsLiked = isLiked;
        album.IsBlocked = isBlocked;
    }

    private void OnTrackInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        if (Track.Id != id) return;
        Track.IsLiked = isLiked;
        Track.IsBlocked = isBlocked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not MediaTrackViewModel vm) return false;
        return vm.Id == Id;
    }

    protected bool Equals(MediaTrackViewModel other)
    {
        return _track.Equals(other._track) && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_track, Id);
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        if (CurrentArtist != null)
        {
            CurrentArtist.IsPlaying = false;
            CurrentArtist.IsCurrent = false;
            CurrentArtist = null;
        }
        if (CurrentAlbum != null)
        {
            CurrentAlbum.IsCurrent = false;
            CurrentAlbum.IsPlaying = false;
            CurrentAlbum = null;
        }
        Track.IsPlaying = false;
        Track.IsCurrent = false;

        var control = App.ServiceProvider.GetRequiredService<RoomViewModel>();
        if (control.IsInRoom)
        {
            if(Track.Id != trackId) return;
            Track.IsCurrent = true;
            Track.IsPlaying = isPlaying;
            return;
        }
        
        switch (containerType)
        {
            case ContainerType.Track:
            case ContainerType.Liked:
                if(trackId != Track.Id) return;
                Track.IsCurrent = true;
                Track.IsPlaying = isPlaying;
                return;
            case ContainerType.Artist:
                var artist = Artists.FirstOrDefault(x => x.Id == containerId);
                if(artist == null) return;
                CurrentArtist = artist;
                CurrentArtist.IsPlaying = isPlaying;
                CurrentArtist.IsCurrent = true;
                
                if(trackId != Track.Id) return;
                Track.IsCurrent = true;
                Track.IsPlaying = isPlaying;
                
                return;
            case ContainerType.Album:
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if(album == null) return;
                CurrentAlbum = album;
                CurrentAlbum.IsPlaying = isPlaying;
                CurrentAlbum.IsCurrent = true;
                
                if(trackId != Track.Id) return;
                Track.IsCurrent = true;
                Track.IsPlaying = isPlaying;
                return;
            default:
                return;
        }
    }

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

    private IArtist? _currentArtist;
    private IAlbum? _currentAlbum;

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