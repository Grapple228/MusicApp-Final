using Music.Services.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Services.DTOs.Extensions.Converters;

public static class PlaylistConverter
{
    public static PlaylistShortDto AsShortDto(this IPlaylistBase playlist, UserShort? user,
        IEnumerable<TrackShort> tracks, ImagePath imagePath) =>
        new(playlist.Id, playlist.Title, user, imagePath.GetPath(playlist.Id), tracks, playlist.IsPublic);

    public static PlaylistDto AsDto(this IPlaylistBase playlist, UserShortDto? user,
        IEnumerable<TrackShortDto> tracks, ImagePath imagePath) =>
        new(playlist.Id, playlist.Title, user, imagePath.GetPath(playlist.Id), tracks, playlist.IsPublic);
}