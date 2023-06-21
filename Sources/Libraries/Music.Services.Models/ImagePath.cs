using Music.Shared.Services;

namespace Music.Services.Models;

public record ImagePath(string ServiceName, string GatewayPath);

public static class Extensions
{
    public static string GetPath(this ImagePath imagePath, Guid id) 
        => $"{imagePath.GatewayPath}/api/{ServiceNames.Images}/{imagePath.ServiceName}/{id}";
}