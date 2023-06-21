namespace Music.Shared.Files.Helpers;

public static class ExtensionHelpers
{
    public static IEnumerable<string> GetAllowedExtensions(this FileTypeEnum type)
        => type switch
        {
            FileTypeEnum.Audio => new [] { FileExtensions.Mp3 },
            FileTypeEnum.Image => new [] { FileExtensions.Png, FileExtensions.Jpg},
            _ => throw new Exception($"Type '{type}' is invalid!")
        };
    
    public static string GetDataType(this FileTypeEnum type)
        => type switch
        {
            FileTypeEnum.Audio => "audio/mpeg",
            FileTypeEnum.Image => "image/jpeg",
            _ => throw new Exception($"Type '{type}' is invalid!")
        };
    
    public static string GetExtension(this FileTypeEnum type) => 
        type switch
        {
            FileTypeEnum.Image => FileExtensions.Jpg,
            FileTypeEnum.Audio => FileExtensions.Mp3,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}