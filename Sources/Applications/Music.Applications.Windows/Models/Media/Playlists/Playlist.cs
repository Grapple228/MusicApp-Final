using System.Collections.ObjectModel;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Users;

namespace Music.Applications.Windows.Models.Media.Playlists;

public class Playlist : MediaModel, IPlaylist
{
    public Playlist(PlaylistDto dto) : base(dto, dto.Id, ChangeImage.Playlists)
    {
        Title = dto.Title;
        Owner = dto.User;
        IsUserOwner = ApplicationService.IsUserOwner(Owner?.Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
        foreach (var track in dto.Tracks) Tracks.Add(new TrackShort(track));
        var genres = dto.Tracks.SelectMany(x => x.Genres).Distinct();
        Genres = new ObservableCollection<GenreDto>(genres);
    }

    public Playlist(MediaPlaylistDto dto) : base(dto, dto.Id, ChangeImage.Playlists)
    {
        Title = dto.Title;
        IsLiked = dto.IsLiked;
        IsBlocked = dto.IsBlocked;
        Owner = dto.User;
        IsUserOwner = ApplicationService.IsUserOwner(Owner?.Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
        foreach (var track in dto.Tracks) Tracks.Add(new TrackShort(track));
        var genres = dto.Tracks.SelectMany(x => x.Genres).Distinct();
        Genres = new ObservableCollection<GenreDto>(genres);
    }

    public UserShortDto? Owner { get; }
    public ObservableCollection<TrackShort> Tracks { get; } = new();
    public ObservableCollection<GenreDto> Genres { get; }

    public string Title { get; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }

    public override async void ChangeLiked()
    {
        base.ChangeLiked();
        await MediaEvents.ChangePlaylistInfo(Id, IsLiked, IsBlocked);
    }

    public override async void ChangeBlocked()
    {
        base.ChangeBlocked();
        await MediaEvents.ChangePlaylistInfo(Id, IsLiked, IsBlocked);
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