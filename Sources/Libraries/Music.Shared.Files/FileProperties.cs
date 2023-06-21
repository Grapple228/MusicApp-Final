namespace Music.Shared.Files;

public class FileProperties
{
    /// <summary>
    /// Имя файла с расширением
    /// </summary>
    public string FileName { get; }
    
    /// <summary>
    /// Имя файла без расширения
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Расширение файла
    /// </summary>
    public string Extension { get;  }

    public FileProperties(string path)
    {
        Name = Path.GetFileNameWithoutExtension(path);
        Extension = Path.GetExtension(path);
        FileName = $"{Name}{Extension}";
    }
}