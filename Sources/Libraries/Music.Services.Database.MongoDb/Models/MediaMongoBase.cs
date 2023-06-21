using Music.Services.Models;
using Music.Services.Models.Media;

namespace Music.Services.Database.MongoDb.Models;

public interface IMediaMongo : IMediaModelBase
{
}

public class MediaMongoBase : MediaModelBase, IMediaMongo
{
}