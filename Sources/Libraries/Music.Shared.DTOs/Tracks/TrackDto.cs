using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Genres;

namespace Music.Shared.DTOs.Tracks;

public record TrackDto(
    Guid Id,
    string Title,
    int Duration,
    DateOnly PublicationDate,
    string ImagePath,
    ArtistShort Owner,
    IEnumerable<GenreDto> Genres,
    IEnumerable<ArtistShortDto> Artists,
    IEnumerable<AlbumShortDto> Albums,
    bool IsPublic) : 
    ImageRecordBase(ImagePath), IModel;