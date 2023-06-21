﻿using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Short;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;
using AlbumShort = Music.Applications.Windows.Models.Media.Albums.AlbumShort;
using TrackShort = Music.Applications.Windows.Models.Media.Tracks.TrackShort;

namespace Music.Applications.Windows.ViewModels.Media.Artist;

public class MediaArtistViewModel : LoadableViewModel, IDisplaying, IOwnerable
{
    private Models.Media.Artists.Artist _artist = null!;
    private bool _isSecondaryOwner;

    private bool _isUserOwner;

    public MediaArtistViewModel(Guid artistId)
    {
        Id = artistId;
        Task.Run(() => LoadInfo(artistId));
        MediaEvents.AlbumInfoChanged += OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged += OnArtistInfoChanged;

        AlbumEvents.AlbumDeleted += AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoveRequested += AlbumEventsOnAlbumRemoveRequested;
        AlbumEvents.AlbumRemoved += AlbumEventsOnAlbumRemoved;
        AlbumEvents.AddedToAlbum += AlbumEventsOnAddedToAlbum;
        AlbumEvents.AlbumCreated += AlbumEventsOnAlbumCreated;

        TrackEvents.TrackDeleted += TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested += TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved += TrackEventsOnTrackRemoved;
        TrackEvents.TrackCreated += TrackEventsOnTrackCreated;
        TrackEvents.TrackAdded += TrackEventsOnTrackAdded;
        TrackEvents.DurationChanged += TrackEventsOnDurationChanged;

        ArtistEvents.ArtistRemoved += ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded += ArtistEventsOnArtistAdded;
        
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public static ContainerType ContainerType => ContainerType.Artist;

    public override string ModelName { get; protected set; } = "Artist";

    public ObservableCollection<AlbumShort> Albums => Artist.Albums;
    public ObservableCollection<TrackShort> Tracks => Artist.Tracks;

    public Models.Media.Artists.Artist Artist
    {
        get => _artist;
        set
        {
            if (Equals(value, _artist)) return;
            _artist = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Tracks));
            OnPropertyChanged(nameof(Albums));
            CurrentDisplaying = DisplayingMedia.ArtistViewModel;
            IsUserOwner = Artist.IsUserOwner;
            IsSecondaryOwner = Artist.IsSecondaryOwner;
            IsRemovable = Artist.IsRemovable;
        }
    }

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.ArtistViewModel;
        set
        {
            DisplayingMedia.ArtistViewModel = value;
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

    private void AlbumEventsOnAlbumCreated(AlbumDto albumDto)
    {
        if (Id != albumDto.Owner.Id || albumDto.Artists.All(a => a.Id != Id)) return;
        var album = Albums.FirstOrDefault(x => x.Id == albumDto.Id);
        if (album != null) return;
        ApplicationService.Invoke(() => Albums.Add(new AlbumShort(albumDto)));
    }

    private void AlbumEventsOnAlbumRemoved(Guid albumId, ContainerType containerType, Guid containerId)
    {
        if (ContainerType != containerType || containerId != Id) return;
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
        if (ContainerType != containerType || containerId != Id) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
    }

    private void TrackEventsOnTrackRemoveRequested(Guid trackId, ContainerType containerType)
    {
        if (containerType != ContainerType) return;
        if (Tracks.FirstOrDefault(x => x.Id == trackId) == null) return;
        TrackEvents.Remove(trackId, containerType, Id);
    }

    private void TrackEventsOnTrackDeleted(Guid trackId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
    }

    private void AlbumEventsOnAlbumDeleted(Guid albumId)
    {
        var tracks = Tracks.Where(x => x.Albums.Any(a => a.Id == albumId));

        foreach (var trackShort in tracks)
        {
            var albumToRemove = trackShort.Albums.First(x => x.Id == albumId);
            ApplicationService.Invoke(() => trackShort.Albums.Remove(albumToRemove));
        }

        var album = Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => Albums.Remove(album));
    }

    public override void Dispose()
    {
        MediaEvents.AlbumInfoChanged -= OnAlbumInfoChanged;
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;
        MediaEvents.ArtistInfoChanged -= OnArtistInfoChanged;

        AlbumEvents.AlbumDeleted -= AlbumEventsOnAlbumDeleted;
        AlbumEvents.AlbumRemoveRequested -= AlbumEventsOnAlbumRemoveRequested;
        AlbumEvents.AlbumRemoved -= AlbumEventsOnAlbumRemoved;
        AlbumEvents.AddedToAlbum -= AlbumEventsOnAddedToAlbum;
        AlbumEvents.AlbumCreated -= AlbumEventsOnAlbumCreated;

        TrackEvents.TrackDeleted -= TrackEventsOnTrackDeleted;
        TrackEvents.TrackRemoveRequested -= TrackEventsOnTrackRemoveRequested;
        TrackEvents.TrackRemoved -= TrackEventsOnTrackRemoved;
        TrackEvents.TrackCreated -= TrackEventsOnTrackCreated;
        TrackEvents.TrackAdded -= TrackEventsOnTrackAdded;
        TrackEvents.DurationChanged -= TrackEventsOnDurationChanged;

        ArtistEvents.ArtistRemoved -= ArtistEventsOnArtistRemoved;
        ArtistEvents.ArtistAdded -= ArtistEventsOnArtistAdded;
        
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    private void TrackEventsOnDurationChanged(Guid trackId, int duration)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => track.Duration = duration);
    }

    private void TrackEventsOnTrackAdded(Guid trackId, Guid mediaId, ContainerType containerType)
    {
        if (containerType != ContainerType.Artist) return;
        if (mediaId != Id) return;
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track != null) return;
        Task.Run(async () => await Reload());
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
                var track = Tracks.FirstOrDefault(x => x.Id == mediaId);
                if (track == null) return;
                if (track.Artists.Any(x => x.Id == artistId)) return;
                break;
            default:
                return;
        }

        Task.Run(async () => await Reload());
    }

    private void ArtistEventsOnArtistRemoved(Guid artistId, ContainerType containerType, Guid containerId)
    {
        CustomArtistShort? artist;

        switch (containerType)
        {
            case ContainerType.Album:
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if (album == null) return;
                artist = album.Artists.FirstOrDefault(x => x.Id == artistId);
                if (artist == null) return;
                ApplicationService.Invoke(() => album.Artists.Remove(artist));
                break;
            case ContainerType.Track:
                var track = Tracks.FirstOrDefault(x => x.Id == containerId);
                if (track == null) return;
                artist = track.Artists.FirstOrDefault(x => x.Id == artistId);
                if (artist == null) return;
                ApplicationService.Invoke(() => track.Artists.Remove(artist));
                break;
        }
    }

    private void AlbumEventsOnAddedToAlbum(Guid albumId, Guid mediaId, ContainerType containerType)
    {
        switch (containerType)
        {
            case ContainerType.Artist:
                if (Id != mediaId) return;
                if (Albums.FirstOrDefault(x => x.Id == albumId) != null) return;
                break;
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

    private void TrackEventsOnTrackCreated(TrackDto track)
    {
        if (track.Owner.Id != Id) return;
        var existing = Tracks.FirstOrDefault(x => x.Id == track.Id);
        if (existing != null) return;
        ApplicationService.Invoke(() => Tracks.Add(new TrackShort(track)));
    }

    ~MediaArtistViewModel()
    {
        Dispose();
    }

    protected override async Task LoadMedia(Guid artistId)
    {
        var artist = await App.GetService().GetMediaArtist(artistId);
        Artist = new Models.Media.Artists.Artist(artist);
        await MediaEvents.ChangeArtistInfo(Artist.Id, Artist.IsLiked, Artist.IsBlocked, false);
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Id);
    }

    private void OnArtistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        if (Artist.Id != id) return;
        Artist.IsLiked = isLiked;
        Artist.IsBlocked = isBlocked;
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
        var track = Tracks.FirstOrDefault(x => x.Id == id);
        if (track == null) return;

        track.IsLiked = isLiked;
        track.IsBlocked = isBlocked;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not MediaArtistViewModel vm) return false;
        return vm.Id == Id;
    }

    protected bool Equals(MediaArtistViewModel other)
    {
        return _artist.Equals(other._artist) && _isUserOwner == other._isUserOwner && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        if (CurrentTrack != null)
        {
            CurrentTrack.IsPlaying = false;
            CurrentTrack.IsCurrent = false;
            CurrentTrack = null;
        }
        if (CurrentAlbum != null)
        {
            CurrentAlbum.IsCurrent = false;
            CurrentAlbum.IsPlaying = false;
            CurrentAlbum = null;
        }
        Artist.IsPlaying = false;
        Artist.IsCurrent = false;

        switch (containerType)
        {
            case ContainerType.Artist:
                if(containerId != Id) return;
                var track = Tracks.FirstOrDefault(x => x.Id == trackId);
                if(track == null) return;
                CurrentTrack = track;
                CurrentTrack.IsPlaying = Artist.IsPlaying = isPlaying;
                CurrentTrack.IsCurrent = Artist.IsCurrent = true;
                return;
            case ContainerType.Album:
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if(album == null) return;
                CurrentAlbum = album;
                CurrentAlbum.IsPlaying = isPlaying;
                CurrentAlbum.IsCurrent = true;
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

    private ITrack? _currentTrack;
    private IAlbum? _currentAlbum;

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