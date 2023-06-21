using Music.Services.Models;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Services.DTOs.Extensions;

public static class ShortHelper
{
    public static AlbumShort AsShort(this IAlbumBase album) =>
        new(album.Id, album.Title, album.PublicationDate, album.OwnerId);

    public static ArtistShort AsShort(this IArtistBase artist) =>
        new(artist.Id, artist.Name);

    public static TrackShort AsShort(this ITrackBase track) =>
        new(track.Id, track.Title, track.PublicationDate, track.OwnerId);

    public static PlaylistShort AsShort(this IPlaylistBase playlist) =>
        new(playlist.Id, playlist.Title, playlist.OwnerId);

    public static UserShort AsShort(this IUserBase user) =>
        new(user.Id, user.Username);

    public static IEnumerable<AlbumShort> AsShort(this IEnumerable<IAlbumBase> albums) =>
        albums.Select(AsShort);
    public static IEnumerable<ArtistShort> AsShort(this IEnumerable<IArtistBase> artists) =>
        artists.Select(AsShort);
    public static IEnumerable<TrackShort> AsShort(this IEnumerable<ITrackBase> tracks) =>
        tracks.Select(AsShort);
    public static IEnumerable<PlaylistShort> AsShort(this IEnumerable<IPlaylistBase> playlists) =>
        playlists.Select(AsShort);
    public static IEnumerable<UserShort> AsShort(this IEnumerable<IUserBase> users) =>
        users.Select(AsShort);
}