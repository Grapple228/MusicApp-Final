using Music.Services.Common;

namespace Music.Services.Files;

public class FileProviderSettings : ISettings
{
    /// <summary>
    /// Size in bytes
    /// </summary>
    public int MaxFileSize { get; init; }

    private IReadOnlyCollection<string> _allowedExtensions = null!;

    public IReadOnlyCollection<string> AllowedExtensions
    {
        get => _allowedExtensions;
        set
        {
            if (value == null || value.Count == 0)
                throw new ArgumentException("Extensions empty");
            
            var ext = new List<string>();
            
            foreach (var e in value)
            {
                if(e.Length == 0)
                    throw new ArgumentException("Invalid extension");
                
                if(e.Split('.').Length is not (1 or 2))
                    throw new ArgumentException("Invalid extension");
                
                ext.Add(e[0] == '.' ? e : $".{e}");
            }

            if(ext.Count != ext.Distinct().Count())
                throw new ArgumentException("Extension duplicates");
            
            _allowedExtensions = ext;
        }
    }
    private readonly string _rootPath = null!;
    public string RootPath
    {
        get => _rootPath;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("RootPath is invalid!");
            _rootPath = value.Replace("\\", "/");
        } 
    }
}