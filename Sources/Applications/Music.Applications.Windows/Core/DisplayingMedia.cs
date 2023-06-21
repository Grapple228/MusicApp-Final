using Music.Applications.Windows.Enums;

namespace Music.Applications.Windows.Core;

public static class DisplayingMedia
{
    public static CurrentDisplaying TrackViewModel { get; set; } = CurrentDisplaying.Albums;
    public static CurrentDisplaying AlbumViewModel { get; set; } = CurrentDisplaying.Tracks;
    public static CurrentDisplaying ArtistViewModel { get; set; } = CurrentDisplaying.Tracks;
    public static CurrentDisplaying LikedViewModel { get; set; } = CurrentDisplaying.Tracks;
    public static CurrentDisplaying BrowseViewModel { get; set; } = CurrentDisplaying.Tracks;
    public static CurrentDisplaying ArtistStudioViewModel { get; set; } = CurrentDisplaying.Tracks;
    public static CurrentDisplaying AdminPanelViewModel { get; set; } = CurrentDisplaying.Users;
}