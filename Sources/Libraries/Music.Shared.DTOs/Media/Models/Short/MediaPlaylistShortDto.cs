using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Media.Models.Short;

public record MediaPlaylistShortDto(
    Guid Id,
    bool IsLiked,
    bool IsBlocked,
    string Title,
    UserShort? User,
    string ImagePath,
    IEnumerable<TrackShort> Tracks): ImageRecordBase(ImagePath), IModel;