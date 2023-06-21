using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Playlists;

public record PlaylistDto(
    Guid Id,
    string Title,
    UserShortDto? User,
    string ImagePath,
    IEnumerable<TrackShortDto> Tracks,
    bool IsPublic) : ImageRecordBase(ImagePath), IModel;