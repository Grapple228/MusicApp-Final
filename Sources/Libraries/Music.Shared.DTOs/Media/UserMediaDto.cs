using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Media;

public record UserMediaDto(
        UserShortDto User,
        IEnumerable<MediaAlbumDto> Albums,
        IEnumerable<MediaTrackDto> Tracks,
        IEnumerable<MediaPlaylistDto> Playlists,
        IEnumerable<MediaArtistDto> Artists);