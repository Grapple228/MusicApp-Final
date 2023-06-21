using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface IPlaylistMongo : IPlaylistBase
{
    ICollection<Guid> Tracks { get; set; }
}

public class PlaylistMongoBase : PlaylistBase, IPlaylistMongo
{
    public ICollection<Guid> Tracks { get; set; } = new List<Guid>();
}