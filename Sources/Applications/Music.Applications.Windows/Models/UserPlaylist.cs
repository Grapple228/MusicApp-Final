using Music.Applications.Windows.Core;
using Music.Shared.DTOs.Playlists;

namespace Music.Applications.Windows.Models;

public class UserPlaylist : ObservableObject
{
    private string _title = null!;

    public UserPlaylist(PlaylistDto playlistDto)
    {
        Id = playlistDto.Id;
        Title = playlistDto.Title;
    }

    public UserPlaylist(Guid id, string title)
    {
        Id = id;
        Title = title;
    }

    public Guid Id { get; init; }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }
}