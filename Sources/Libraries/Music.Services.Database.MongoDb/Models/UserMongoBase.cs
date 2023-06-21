using Music.Services.Models;

namespace Music.Services.Database.MongoDb.Models;

public interface IUserMongo : IUserBase
{
    ICollection<Guid> Playlists { get; set; }
}

public class UserMongoBase : UserBase, IUserMongo
{
    public ICollection<Guid> Playlists { get; set; } = new List<Guid>();
}