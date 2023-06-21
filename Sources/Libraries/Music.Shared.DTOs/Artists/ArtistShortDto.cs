using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Artists;

public record ArtistShortDto(
    Guid Id,
    string Name,
    string ImagePath,
    IEnumerable<AlbumShort> Albums,
    IEnumerable<TrackShort> Tracks
) : ImageRecordBase(ImagePath), IModel;