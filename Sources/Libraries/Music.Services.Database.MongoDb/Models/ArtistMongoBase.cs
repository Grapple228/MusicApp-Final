using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface IArtistMongo : IArtistBase
{
}

public class ArtistMongoBase : ArtistBase, IArtistMongo
{
}