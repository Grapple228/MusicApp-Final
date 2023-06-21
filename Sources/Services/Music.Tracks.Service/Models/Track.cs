using Music.Services.Database.MongoDb.Models;

namespace Music.Tracks.Service.Models;

public class Track : TrackMongoBase
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}