namespace Music.Applications.Windows.Events;

public delegate void InfoHandler(Guid id, bool isLiked, bool isBlocked);

public static class MediaEvents
{
    public static async Task ChangeTrackInfo(Guid trackId, bool isLiked, bool isBlocked, bool isServerRequest = true)
    {
        if (isServerRequest)
            try
            {
                await App.GetService().ChangeTrackInfo(trackId, isLiked, isBlocked);
            }
            catch
            {
                return;
            }

        TrackInfoChanged?.Invoke(trackId, isLiked, isBlocked);
    }

    public static async Task ChangeArtistInfo(Guid artistId, bool isLiked, bool isBlocked, bool isServerRequest = true)
    {
        if (isServerRequest)
            try
            {
                await App.GetService().ChangeArtistInfo(artistId, isLiked, isBlocked);
            }
            catch
            {
                return;
            }

        ArtistInfoChanged?.Invoke(artistId, isLiked, isBlocked);
    }

    public static async Task ChangeAlbumInfo(Guid albumId, bool isLiked, bool isBlocked, bool isServerRequest = true)
    {
        if (isServerRequest)
            try
            {
                await App.GetService().ChangeAlbumInfo(albumId, isLiked, isBlocked);
            }
            catch
            {
                return;
            }

        AlbumInfoChanged?.Invoke(albumId, isLiked, isBlocked);
    }

    public static async Task ChangePlaylistInfo(Guid playlistId, bool isLiked, bool isBlocked,
        bool isServerRequest = true)
    {
        if (isServerRequest)
            try
            {
                await App.GetService().ChangePlaylistInfo(playlistId, isLiked, isBlocked);
            }
            catch
            {
                return;
            }

        PlaylistInfoChanged?.Invoke(playlistId, isLiked, isBlocked);
    }

    public static event InfoHandler? TrackInfoChanged;
    public static event InfoHandler? ArtistInfoChanged;
    public static event InfoHandler? AlbumInfoChanged;
    public static event InfoHandler? PlaylistInfoChanged;
}