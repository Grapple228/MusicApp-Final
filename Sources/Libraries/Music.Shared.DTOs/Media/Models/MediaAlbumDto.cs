using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Media.Models.Short;

namespace Music.Shared.DTOs.Media.Models;

public record MediaAlbumDto(
    Guid Id,
    bool IsLiked, 
    bool IsBlocked,
    string Title, 
    DateOnly PublicationDate, 
    string ImagePath,
    ArtistShort Owner,
    IEnumerable<MediaArtistShortDto> Artists,
    IEnumerable<MediaTrackShortDto> Tracks) : 
    ImageRecordBase(ImagePath), IModel;