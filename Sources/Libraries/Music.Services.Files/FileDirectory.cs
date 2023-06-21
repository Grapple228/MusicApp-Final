using Music.Shared.Common;

namespace Music.Services.Files;

public class FileDirectory : IModel
{
    public Guid Id { get; init; }
    public string RootPath { get; set; } = null!;
    public ICollection<FileInfoModel> Files { get; set; } = new List<FileInfoModel>();
}