using Music.Services.Database.MongoDb.Models;

namespace Music.Genres.Service.Models;

public class Genre : GenreMongoBase
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}