using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Playlists;

namespace Music.Shared.DTOs.Users;

public record UserShortDto(
    Guid Id,
    string Username,
    string ImagePath) : 
    ImageRecordBase(ImagePath), IModel;