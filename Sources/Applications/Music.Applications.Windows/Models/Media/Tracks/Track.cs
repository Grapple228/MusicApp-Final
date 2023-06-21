using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Tools.Helpers;

namespace Music.Applications.Windows.Models.Media.Tracks;

public class Track : MediaModel, ITrack
{
    public Track(TrackDto dto) : base(dto, dto.Id, ChangeImage.Tracks)
    {
        Title = dto.Title;
        PublicationDate = dto.PublicationDate;
        Duration = dto.Duration;
        DurationString = dto.Duration.GetTimeString();
        OwnerId = dto.Owner.Id;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        IsRemovable = !IsUserOwner;
        foreach (var artist in dto.Artists) Artists.Add(new ArtistShort(artist, OwnerId == artist.Id));
        foreach (var album in dto.Albums) Albums.Add(new AlbumShort(album));
        foreach (var genre in dto.Genres) Genres.Add(genre);
    }

    public Track(MediaTrackDto dto) : base(dto, dto.Id, ChangeImage.Tracks)
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
        foreach (var artist in dto.Artists) Artists.Add(new ArtistShort(artist, OwnerId == artist.Id));
        foreach (var album in dto.Albums) Albums.Add(new AlbumShort(album));
        foreach (var genre in dto.Genres) Genres.Add(genre);
    }

    public ObservableCollection<AlbumShort> Albums { get; } = new();
    public ObservableCollection<ArtistShort> Artists { get; } = new();

    public string Title { get; init; }
    public Guid OwnerId { get; init; }
    public DateOnly PublicationDate { get; init; }
    public int Duration { get; set; }
    public string DurationString { get; set; }
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