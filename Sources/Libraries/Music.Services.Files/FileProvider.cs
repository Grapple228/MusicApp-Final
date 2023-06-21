using Microsoft.AspNetCore.Mvc;

namespace Music.Services.Files.Providers;

public class FileProvider
{
    private string RootPath { get; } 
    public FileProvider(string rootPath)
    {
        RootPath = rootPath;
    }

    public async Task WriteFile(string fileName, byte[] data)
    {
        if (!Directory.Exists(RootPath))
            Directory.CreateDirectory(RootPath);
        
        await File.WriteAllBytesAsync($"{RootPath}/{fileName}", data);
    }

    public async Task WriteFile(string fileName, Stream stream)
    {
        if (!Directory.Exists(RootPath))
            Directory.CreateDirectory(RootPath);
        
        await using var fileStream = new FileStream($"{RootPath}/{fileName}", FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }

    public PhysicalFileResult ReadFile(string fileName, string dataType)
    {
        return new PhysicalFileResult($"{RootPath}/{fileName}", dataType);
    }

    public void DeleteFile(string fileName)
    {
        if(!Directory.Exists(RootPath))
            return;
        var filePath = $"{RootPath}/{fileName}";
        if(!File.Exists(filePath)) return;
        File.Delete(filePath);
    }
}