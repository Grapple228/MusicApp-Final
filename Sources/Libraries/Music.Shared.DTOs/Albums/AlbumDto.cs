using Music.Shared.Common;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Albums;

public record AlbumDto(
    Guid Id,
    string Title,
    DateOnly PublicationDate,
    string ImagePath,
    ArtistShort Owner,
    IEnumerable<ArtistShortDto> Artists,
    IEnumerable<TrackShortDto> Tracks,
    bool IsPublic) : 
    ImageRecordBase(ImagePath), IModel;
    