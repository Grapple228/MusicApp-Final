using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface ITrackMongo : ITrackBase
{
    ICollection<Guid> Genres { get; set; }
    ICollection<Guid> Artists { get; set; }
    ICollection<Guid> Albums { get; set; }
}

public class TrackMongoBase : TrackBase, ITrackMongo
{
    public ICollection<Guid> Genres { get; set; } = new List<Guid>();
    public ICollection<Guid> Artists { get; set; } = new List<Guid>();
    public ICollection<Guid> Albums { get; set; } = new List<Guid>();
}