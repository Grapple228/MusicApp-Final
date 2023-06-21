using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Media.Models;

namespace Music.Applications.Windows.Models.Media.Artists;

public class Artist : MediaModel, IArtist
{
    public Artist(ArtistDto dto) : base(dto, dto.Id, ChangeImage.Artists)
    {
        Name = dto.Name;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !ApplicationService.IsSame(Id);
        foreach (var album in dto.Albums) Albums.Add(new AlbumShort(album));
        foreach (var track in dto.Tracks) Tracks.Add(new TrackShort(track));
    }

    public Artist(MediaArtistDto dto) : base(dto, dto.Id, ChangeImage.Artists)
    {
        Name = dto.Name;
        IsLiked = dto.IsLiked;
        IsBlocked = dto.IsBlocked;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !ApplicationService.IsSame(Id);
        foreach (var album in dto.Albums) Albums.Add(new AlbumShort(album));
        foreach (var track in dto.Tracks) Tracks.Add(new TrackShort(track));
    }

    public ObservableCollection<AlbumShort> Albums { get; } = new();
    public ObservableCollection<TrackShort> Tracks { get; } = new();

    public string Name { get; set; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }

    public override async void ChangeLiked()
    {
        base.ChangeLiked();
        await MediaEvents.ChangeArtistInfo(Id, IsLiked, IsBlocked);
    }

    public override async void ChangeBlocked()
    {
        base.ChangeBlocked();
        await MediaEvents.ChangeArtistInfo(Id, IsLiked, IsBlocked);
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