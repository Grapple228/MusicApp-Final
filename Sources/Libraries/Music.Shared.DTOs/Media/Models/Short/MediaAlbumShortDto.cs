using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Media.Models.Short;

public record MediaAlbumShortDto(
    Guid Id,
    bool IsLiked,
    bool IsBlocked,
    string Title,
    DateOnly PublicationDate,
    string ImagePath,
    ArtistShort Owner,
    IEnumerable<ArtistShort> Artists,
    IEnumerable<TrackShort> Tracks) 
    : ImageRecordBase(ImagePath), IModel;