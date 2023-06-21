using Music.Services.Database.MongoDb.Models;

namespace Music.Albums.Service.Models;

public class Album : AlbumMongoBase
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}