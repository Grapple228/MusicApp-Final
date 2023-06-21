using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;

namespace Music.Albums.Service.Models;

public class CustomAlbumInfo : AlbumInfo
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}