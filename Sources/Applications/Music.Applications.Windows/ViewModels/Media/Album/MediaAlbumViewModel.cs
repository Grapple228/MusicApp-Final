using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Album;

public class MediaAlbumViewModel : LoadableViewModel, IDisplaying, IOwnerable
{
    private Models.Media.Albums.Album _album = null!;
    private bool _isSecondaryOwner;

    private bool _isUserOwner;
    private ITrack? _currentTrack;
    private IArtist? _currentArtist;

    public MediaAlbumViewModel(Guid albumId)
    {
        Id = albumId;
        Task.Run(() => LoadInfo(albumId));
        MediaEvents.AlbumInfoChanged += OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged += OnArtistInfoChanged;

        TrackEvents.TrackDeleted += TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested += TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved += TrackEventsOnTrackRemoved;
        TrackEvents.TrackAdded += TrackEventsOnTrackAdded;
        TrackEvents.GenreAdded += TrackEventsOnGenreAdded;
        TrackEvents.GenreRemoved += TrackEventsOnGenreRemoved;
        TrackEvents.DurationChanged += TrackEventsOnDurationChanged;

        AlbumEvents.AddedToAlbum += AlbumEventsOnAddedToAlbum;

        ArtistEvents.ArtistRemoveRequested += ArtistEventsOnArtistRemoveRequested;
        ArtistEvents.ArtistRemoved += ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded += ArtistEventsOnArtistAdded;
        
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public static ContainerType ContainerType => ContainerType.Album;

    public override string ModelName { get; protected set; } = "Album";

    public ObservableCollection<ArtistShort> Artists => Album.Artists;
    public ObservableCollection<TrackShort> Tracks => Album.Tracks;
    public ObservableCollection<Models.Media.Genres.Genre> Genres { get; } = new();

    public Models.Media.Albums.Album Album
    {
        get => _album;
        private set
        {
            if (Equals(value, _album)) return;
            _album = value;
            OnPropertyChanged(nameof(Artists));
            OnPropertyChanged(nameof(Tracks));
            OnPropertyChanged(nameof(Genres));
            OnPropertyChanged();
            IsUserOwner = Album.IsUserOwner;
            IsSecondaryOwner = Album.IsSecondaryOwner;
            IsRemovable = Album.IsRemovable;
            UpdateGenres();
        }
    }

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.AlbumViewModel;
        set
        {
            DisplayingMedia.AlbumViewModel = value;
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

    private void TrackEventsOnDurationChanged(Guid trackId, int duration)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => track.Duration = duration);
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

    private void TrackEventsOnTrackAdded(Guid trackId, Guid mediaId, ContainerType containerType)
    {
        if (containerType != ContainerType.Album) return;
        if (mediaId != Id) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track != null) return;
        Task.Run(async () => await Reload());
    }

    private void AlbumEventsOnAddedToAlbum(Guid albumId, Guid mediaId, ContainerType containerType)
    {
        switch (containerType)
        {
            case ContainerType.Track:
                var track = Tracks.FirstOrDefault(x => x.Id == mediaId);
                if (track == null) return;
                if (track.Albums.Any(x => x.Id == albumId)) return;
                break;
            default:
                return;
        }

        Task.Run(async () => await Reload());
    }

    private void ArtistEventsOnArtistRemoved(Guid artistId, ContainerType containerType, Guid containerId)
    {
        if (ContainerType != containerType || containerId != Id) return;
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

    private void TrackEventsOnTrackDeleted(Guid trackId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
    }

    public override void Dispose()
    {
        MediaEvents.AlbumInfoChanged -= OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged -= OnArtistInfoChanged;

        AlbumEvents.AddedToAlbum -= AlbumEventsOnAddedToAlbum;

        TrackEvents.TrackDeleted -= TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested -= TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved -= TrackEventsOnTrackRemoved;
        TrackEvents.TrackAdded -= TrackEventsOnTrackAdded;
        TrackEvents.GenreAdded -= TrackEventsOnGenreAdded;
        TrackEvents.GenreRemoved -= TrackEventsOnGenreRemoved;
        TrackEvents.DurationChanged -= TrackEventsOnDurationChanged;

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
                if (mediaId != Id) return;
                if (Artists.FirstOrDefault(x => x.Id == artistId) != null) return;
                break;
            case ContainerType.Track:
                var track = Tracks.FirstOrDefault(x => x.Id == mediaId);
                if (track == null) return;
                if (track.Artists.Any(x => x.Id == artistId)) return;
                break;
            default:
                return;
        }

        Task.Run(async () => await Reload());
    }

    private void TrackEventsOnTrackRemoved(Guid trackId, ContainerType containerType, Guid containerId)
    {
        if (ContainerType != containerType || containerId != Id) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
        UpdateGenres();
    }

    private void UpdateGenres()
    {
        var genres = Tracks.SelectMany(x => x.Genres).Distinct();
        ApplicationService.Invoke(() => Genres.Clear());
        foreach (var genreDto in genres)
            ApplicationService.Invoke(() => Genres.Add(new Models.Media.Genres.Genre(genreDto)));
    }

    private void TrackEventsOnTrackRemoveRequested(Guid trackId, ContainerType containerType)
    {
        if (containerType != ContainerType) return;
        if (Tracks.FirstOrDefault(x => x.Id == trackId) == null) return;
        TrackEvents.Remove(trackId, containerType, Id);
    }

    ~MediaAlbumViewModel()
    {
        Dispose();
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
        if (Album.Id != id) return;
        Album.IsLiked = isLiked;
        Album.IsBlocked = isBlocked;
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
        if (obj is not MediaAlbumViewModel vm) return false;
        return vm.Id == Id;
    }

    protected bool Equals(MediaAlbumViewModel other)
    {
        return _album.Equals(other._album) && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_album, Id);
    }

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

    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        if (CurrentTrack != null)
        {
            CurrentTrack.IsPlaying = false;
            CurrentTrack.IsCurrent = false;
            CurrentTrack = null;
        }
        if (CurrentArtist != null)
        {
            CurrentArtist.IsCurrent = false;
            CurrentArtist.IsPlaying = false;
            CurrentArtist = null;
        }
        Album.IsPlaying = false;
        Album.IsCurrent = false;

        switch (containerType)
        {
            case ContainerType.Album:
                if(containerId != Id) return;
                var track = Tracks.FirstOrDefault(x => x.Id == trackId);
                if(track == null) return;
                CurrentTrack = track;
                CurrentTrack.IsPlaying = Album.IsPlaying = isPlaying;
                CurrentTrack.IsCurrent = Album.IsCurrent = true;
                return;
            case ContainerType.Artist:
                var artist = Artists.FirstOrDefault(x => x.Id == containerId);
                if(artist == null) return;
                CurrentArtist = artist;
                CurrentArtist.IsPlaying = isPlaying;
                CurrentArtist.IsCurrent = true;
                return;
            default:
                return;
        }
    }

    protected override async Task LoadMedia(Guid albumId)
    {
        var album = await App.GetService().GetMediaAlbum(albumId);
        Album = new Models.Media.Albums.Album(album);
        await MediaEvents.ChangeAlbumInfo(Album.Id, Album.IsLiked, Album.IsBlocked, false);
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }
}