using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.User;
using Music.Services.Database.MongoDb.Models;
namespace Music.Users.Service.Helpers;

public interface IUserNormalizeHelper : IUserNormalizeHelperBase<TrackMongoBase, UserMongoBase, PlaylistMongoBase>
{
    
}

public class UserNormalizeHelper : UserNormalizeHelperBase<TrackMongoBase, UserMongoBase, PlaylistMongoBase>, IUserNormalizeHelper
{
    public UserNormalizeHelper(IRepository<TrackMongoBase> tracksRepository, IRepository<UserMongoBase> usersRepository,
        IRepository<PlaylistMongoBase> playlistsRepository) : base(tracksRepository, usersRepository,
        playlistsRepository)
    {
    }
}