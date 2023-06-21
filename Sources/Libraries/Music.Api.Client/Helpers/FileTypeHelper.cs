using Music.Shared.Files;

namespace MusicClient.Helpers;

public static class FileTypeHelper
{
    public static FileTypeEnum GetFileType(this string path)
    {
        var extension = Path.GetExtension(path);
        return extension switch
        {
            ".mp3" => FileTypeEnum.Audio,
            ".png" => FileTypeEnum.Image,
            ".jpg" => FileTypeEnum.Image,
            ".jpeg" => FileTypeEnum.Image,
            _ => throw new Exception($"Extension '{extension}' is not supported!")
        };
    }
}