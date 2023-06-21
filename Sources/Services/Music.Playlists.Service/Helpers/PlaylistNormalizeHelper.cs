using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Playlist;
using Music.Services.Database.MongoDb.Models;

namespace Music.Playlists.Service.Helpers;

public interface IPlaylistNormalizeHelper : IPlaylistNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase, UserMongoBase>
{
    
}

public class PlaylistNormalizeHelper : PlaylistNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase, UserMongoBase>, IPlaylistNormalizeHelper
{
    public PlaylistNormalizeHelper(IRepository<ArtistMongoBase> artistsRepository,
        IRepository<AlbumMongoBase> albumsRepository, IRepository<TrackMongoBase> tracksRepository,
        IRepository<GenreMongoBase> genresRepository, IRepository<UserMongoBase> usersRepository) : base(
        artistsRepository, albumsRepository, tracksRepository, genresRepository, usersRepository)
    {
    }
}