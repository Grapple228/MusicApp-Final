using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Playlists;

public record PlaylistShortDto(
    Guid Id,
    string Title,
    UserShort? User,
    string ImagePath,
    IEnumerable<TrackShort> Tracks,
    bool IsPublic
) : ImageRecordBase(ImagePath), IModel;