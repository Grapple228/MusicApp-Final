using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface IAlbumMongo : IAlbumBase
{
    public ICollection<Guid> Tracks { get; set; }
    public ICollection<Guid> Artists { get; set; }
}

public class AlbumMongoBase : AlbumBase, IAlbumMongo
{
    public ICollection<Guid> Tracks { get; set; } = new List<Guid>();
    public ICollection<Guid> Artists { get; set; } = new List<Guid>();
}