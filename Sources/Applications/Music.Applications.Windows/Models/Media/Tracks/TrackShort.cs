using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Short;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Media.Models.Short;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Tools.Helpers;

namespace Music.Applications.Windows.Models.Media.Tracks;

public class TrackShort : MediaModel, ITrack
{
    private int _duration;
    private string _durationString;
    private string _title;

    public TrackShort(TrackDto dto) : base(dto, dto.Id, ChangeImage.Tracks)
    {
        Title = dto.Title;
        PublicationDate = dto.PublicationDate;
        Duration = dto.Duration;
        DurationString = dto.Duration.GetTimeString();
        OwnerId = dto.Owner.Id;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        IsRemovable = !IsUserOwner;
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var album in dto.Albums) Albums.Add(new CustomAlbumShort(album));
        foreach (var genre in dto.Genres) Genres.Add(genre);
    }

    public TrackShort(TrackShortDto dto) : base(dto, dto.Id, ChangeImage.Tracks)
    {
        Title = dto.Title;
        PublicationDate = dto.PublicationDate;
        Duration = dto.Duration;
        DurationString = dto.Duration.GetTimeString();
        OwnerId = dto.Owner.Id;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        IsRemovable = !IsUserOwner;
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var album in dto.Albums) Albums.Add(new CustomAlbumShort(album));
        foreach (var genre in dto.Genres) Genres.Add(genre);
    }

    public TrackShort(MediaTrackShortDto dto) : base(dto, dto.Id, ChangeImage.Tracks)
    {
        Title = dto.Title;
        PublicationDate = dto.PublicationDate;
        IsLiked = dto.IsLiked;
        IsBlocked = dto.IsBlocked;
        Duration = dto.Duration;
        DurationString = dto.DurationString;
        OwnerId = dto.Owner.Id;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        IsRemovable = !IsUserOwner;
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var album in dto.Albums) Albums.Add(new CustomAlbumShort(album));
        foreach (var genre in dto.Genres) Genres.Add(genre);
    }

    public ObservableCollection<CustomAlbumShort> Albums { get; } = new();
    public ObservableCollection<CustomArtistShort> Artists { get; } = new();

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public Guid OwnerId { get; init; }
    public DateOnly PublicationDate { get; init; }

    public int Duration
    {
        get => _duration;
        set
        {
            if (value == _duration) return;
            _duration = value;
            DurationString = _duration.GetTimeString();
            OnPropertyChanged();
        }
    }

    public string DurationString
    {
        get => _durationString;
        set
        {
            if (value == _durationString) return;
            _durationString = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<GenreDto> Genres { get; } = new();
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }

    public override async void ChangeLiked()
    {
        base.ChangeLiked();
        await MediaEvents.ChangeTrackInfo(Id, IsLiked, IsBlocked);
    }

    public override async void ChangeBlocked()
    {
        base.ChangeBlocked();
        await MediaEvents.ChangeTrackInfo(Id, IsLiked, IsBlocked);
    }
    
    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value == _isPlaying) return;
            _isPlaying = value;
            OnPropertyChanged();
        }
    }
    
    private bool _isCurrent;
    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            if (value == _isCurrent) return;
            _isCurrent = value;
            OnPropertyChanged();
        }
    }
}