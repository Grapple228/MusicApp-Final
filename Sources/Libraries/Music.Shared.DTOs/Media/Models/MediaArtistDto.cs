using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Media.Models.Short;

namespace Music.Shared.DTOs.Media.Models;

public record MediaArtistDto(
    Guid Id, 
    bool IsLiked, 
    bool IsBlocked,
    string Name, 
    string ImagePath,
    IEnumerable<MediaAlbumShortDto> Albums, 
    IEnumerable<MediaTrackShortDto> Tracks) : 
    ImageRecordBase(ImagePath), IModel;