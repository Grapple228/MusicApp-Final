using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Artists;

public record ArtistDto(
    Guid Id,
    string Name,
    string ImagePath,
    IEnumerable<AlbumShortDto> Albums,
    IEnumerable<TrackShortDto> Tracks) : ImageRecordBase(ImagePath), IModel;