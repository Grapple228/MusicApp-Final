namespace Music.Services.Files;

public class FileInfoModel
{
    public string Name { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string DataType { get; set; }
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ChangingDate { get; set; } = DateTimeOffset.UtcNow;
}