using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Media.Playlist;

public class AddToPlaylistViewModel : ViewModelBase
{
    private readonly AddToPlaylist _addToPlaylist;
    private readonly Guid _mediaId;

    public AddToPlaylistViewModel(Guid mediaId, AddToPlaylist addToPlaylist)
    {
        _mediaId = mediaId;
        _addToPlaylist = addToPlaylist;
        Playlists = new ObservableCollection<UserPlaylist>();
        PlaylistEvents.PlaylistsLoaded += PlaylistEventsOnPlaylistsLoaded;
        PlaylistEvents.AddToPlaylistRequested += PlaylistRequestedEventsOnAddToPlaylistRequested;
        Task.Run(PlaylistEvents.Load);
    }

    public ObservableCollection<UserPlaylist> Playlists { get; }

    public override string ModelName { get; protected set; } = nameof(AddToPlaylistViewModel);

    private async void PlaylistRequestedEventsOnAddToPlaylistRequested(Guid playlistId)
    {
        var appService = App.GetService();
        if (playlistId == _mediaId) return;
        if (_addToPlaylist == AddToPlaylist.Track) await appService.AddTrackToPlaylist(playlistId, _mediaId);
        else await appService.AddTracksToPlaylist(playlistId, _mediaId, _addToPlaylist);
    }

    private void PlaylistEventsOnPlaylistsLoaded(IEnumerable<UserPlaylist> playlists)
    {
        ApplicationService.Invoke(() => Playlists.Clear());

        foreach (var userPlaylist in playlists) ApplicationService.Invoke(() => Playlists.Add(userPlaylist));
    }

    ~AddToPlaylistViewModel()
    {
        Dispose();
    }


    public override void Dispose()
    {
        PlaylistEvents.PlaylistsLoaded -= PlaylistEventsOnPlaylistsLoaded;
        PlaylistEvents.AddToPlaylistRequested -= PlaylistRequestedEventsOnAddToPlaylistRequested;
        base.Dispose();
    }
}