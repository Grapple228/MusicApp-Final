using Music.Services.Database.MongoDb.Models;

namespace Music.Artists.Service.Models;

public class Artist : ArtistMongoBase
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}