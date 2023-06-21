using Music.Services.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Users;

namespace Music.Services.DTOs.Extensions.Converters;

public static class UserConverter
{
    public static UserShortDto AsShortDto(this IUserBase user, ImagePath imagePath) =>
        new(user.Id, user.Username, imagePath.GetPath(user.Id));

    public static UserDto AsDto(this IUserBase user, ImagePath imagePath, IEnumerable<PlaylistShortDto> playlists) =>
        new(user.Id, user.Username, imagePath.GetPath(user.Id), playlists);
}