namespace Music.Applications.Windows.Events;

public delegate void ImageChanged(Guid id);

public enum ChangeImage
{
    Albums,
    Artists,
    Tracks,
    Playlists,
    Users
}

public static class ImageEvents
{
    public static event ImageChanged? AlbumImageChanged;
    public static event ImageChanged? PlaylistImageChanged;
    public static event ImageChanged? ArtistImageChanged;
    public static event ImageChanged? TrackImageChanged;
    public static event ImageChanged? UserImageChanged;

    public static void Update(Guid id, ChangeImage changeImage)
    {
        switch (changeImage)
        {
            case ChangeImage.Albums:
                AlbumImageChanged?.Invoke(id);
                break;
            case ChangeImage.Artists:
                ArtistImageChanged?.Invoke(id);
                break;
            case ChangeImage.Tracks:
                TrackImageChanged?.Invoke(id);
                break;
            case ChangeImage.Playlists:
                PlaylistImageChanged?.Invoke(id);
                break;
            case ChangeImage.Users:
                UserImageChanged?.Invoke(id);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(changeImage), changeImage, null);
        }
    }
}