using Music.Shared.Common;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Albums;

public record AlbumShortDto(
    Guid Id,
    string Title,
    DateOnly PublicationDate,
    string ImagePath,
    ArtistShort Owner,
    IEnumerable<ArtistShort> Artists,
    IEnumerable<TrackShort> Tracks,
    bool IsPublic
) : ImageRecordBase(ImagePath), IModel;
