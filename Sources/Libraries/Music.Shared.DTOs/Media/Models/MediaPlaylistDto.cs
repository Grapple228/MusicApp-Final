using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Media.Models.Short;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Media.Models;

public record MediaPlaylistDto(
    Guid Id, 
    bool IsLiked, 
    bool IsBlocked,
    string Title, 
    UserShortDto? User, 
    string ImagePath, 
    IEnumerable<MediaTrackShortDto> Tracks) : 
    ImageRecordBase(ImagePath), IModel;