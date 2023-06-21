using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Playlists;

namespace Music.Shared.DTOs.Users;

public record UserDto(
    Guid Id,
    string Username,
    string ImagePath, 
    IEnumerable<PlaylistShortDto> Playlists) :  
    ImageRecordBase(ImagePath), IModel;