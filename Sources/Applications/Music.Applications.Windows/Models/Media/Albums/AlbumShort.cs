using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Short;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Media.Models.Short;

namespace Music.Applications.Windows.Models.Media.Albums;

public class AlbumShort : MediaModel, IAlbum
{
    public AlbumShort(AlbumDto dto) : base(dto, dto.Id, ChangeImage.Albums)
    {
        Title = dto.Title;
        OwnerId = dto.Owner.Id;
        PublicationDate = dto.PublicationDate;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsRemovable = !IsUserOwner;
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var track in dto.Tracks) Tracks.Add(new CustomTrackShort(track));
    }

    public AlbumShort(AlbumShortDto dto) : base(dto, dto.Id, ChangeImage.Albums)
    {
        Title = dto.Title;
        OwnerId = dto.Owner.Id;
        PublicationDate = dto.PublicationDate;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsRemovable = !IsUserOwner;
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var track in dto.Tracks) Tracks.Add(new CustomTrackShort(track));
    }

    public AlbumShort(MediaAlbumShortDto dto) : base(dto, dto.Id, ChangeImage.Albums)
    {
        Title = dto.Title;
        OwnerId = dto.Owner.Id;
        PublicationDate = dto.PublicationDate;
        IsLiked = dto.IsLiked;
        IsBlocked = dto.IsBlocked;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsRemovable = !IsUserOwner;
        IsSecondaryOwner = ApplicationService.IsSecondaryOwner(Artists.Select(x => x.Id));
        foreach (var artist in dto.Artists) Artists.Add(new CustomArtistShort(artist, OwnerId == artist.Id));
        foreach (var track in dto.Tracks) Tracks.Add(new CustomTrackShort(track));
    }

    public ObservableCollection<CustomArtistShort> Artists { get; } = new();
    public ObservableCollection<CustomTrackShort> Tracks { get; } = new();

    public string Title { get; init; }
    public Guid OwnerId { get; init; }
    public DateOnly PublicationDate { get; init; }

    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }

    public override async void ChangeLiked()
    {
        base.ChangeLiked();
        await MediaEvents.ChangeAlbumInfo(Id, IsLiked, IsBlocked);
    }

    public override async void ChangeBlocked()
    {
        base.ChangeBlocked();
        await MediaEvents.ChangeAlbumInfo(Id, IsLiked, IsBlocked);
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