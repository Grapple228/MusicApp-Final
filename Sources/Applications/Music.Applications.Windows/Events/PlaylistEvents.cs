using Music.Applications.Windows.Models;

namespace Music.Applications.Windows.Events;

public delegate void PlaylistCreatedHandler(UserPlaylist playlist);

public delegate void PlaylistUpdatedHandler(UserPlaylist playlist);

public delegate void PlaylistDeletedHandler(Guid playlistId);

public delegate void AddToPlaylistRequestedHandler(Guid playlistId);

public delegate void AddedToPlaylistHandler(Guid playlistId, params Guid[] ids);

public delegate void PlaylistsLoadedHandler(IEnumerable<UserPlaylist> playlists);

public static class PlaylistEvents
{
    public static event PlaylistsLoadedHandler? PlaylistsLoaded;
    public static event PlaylistCreatedHandler? PlaylistCreated;
    public static event AddToPlaylistRequestedHandler? AddToPlaylistRequested;
    public static event PlaylistDeletedHandler? PlaylistDeleted;
    public static event PlaylistUpdatedHandler? PlaylistUpdated;
    public static event AddedToPlaylistHandler? AddedToPlaylist;

    public static void AddToPlaylist(Guid playlistId, params Guid[] ids)
    {
        AddedToPlaylist?.Invoke(playlistId, ids);
    }

    public static async Task Load()
    {
        try
        {
            var playlists = (await App.GetService().GetUserPlaylists()).Select(x => new UserPlaylist(x.Id, x.Title));
            PlaylistsLoaded?.Invoke(playlists);
        }
        catch
        {
            // ignored
        }
    }

    public static void Delete(Guid playlistId)
    {
        PlaylistDeleted?.Invoke(playlistId);
    }

    public static void Update(UserPlaylist playlist)
    {
        PlaylistUpdated?.Invoke(playlist);
    }

    public static void Create(UserPlaylist playlist)
    {
        PlaylistCreated?.Invoke(playlist);
    }

    public static void RequestAddToPlaylist(Guid playlistId)
    {
        AddToPlaylistRequested?.Invoke(playlistId);
    }
}