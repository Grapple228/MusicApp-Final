using Music.Shared.DTOs.Media.Enums;

namespace MusicClient.Helpers;

public static class MediaFilterHelper
{
    public static string GetPath(this MediaFilterEnum filterEnum) => 
        filterEnum switch
        {
            MediaFilterEnum.Liked => "liked",
            MediaFilterEnum.Blocked => "blocked",
            _ => ""
        };
}