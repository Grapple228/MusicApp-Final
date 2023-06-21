using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Shared.DTOs.Media.Models.Short;

public record MediaArtistShortDto(
    Guid Id,
    bool IsLiked,
    bool IsBlocked,
    string Name,
    string ImagePath,
    IEnumerable<AlbumShort> Albums,
    IEnumerable<TrackShort> Tracks): ImageRecordBase(ImagePath), IModel;