using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface IGenreMongo : IGenreBase
{
}

public class GenreMongoBase : GenreBase, IGenreMongo
{
}