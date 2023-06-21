using Music.Shared.Common;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Media.Models.Short;

namespace Music.Shared.DTOs.Media.Models;

public record MediaTrackDto(
    Guid Id, 
    bool IsLiked, 
    bool IsBlocked,
    string Title, 
    int Duration, 
    string DurationString, 
    DateOnly PublicationDate, 
    string ImagePath, 
    ArtistShort Owner,
    IEnumerable<GenreDto> Genres,
    IEnumerable<MediaArtistShortDto> Artists, 
    IEnumerable<MediaAlbumShortDto> Albums) : 
    ImageRecordBase(ImagePath), IModel;