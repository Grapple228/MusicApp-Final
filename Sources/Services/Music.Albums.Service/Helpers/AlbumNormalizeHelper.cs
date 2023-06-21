using Music.Albums.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Models;

namespace Music.Albums.Service.Helpers;

public interface IAlbumNormalizeHelper : IAlbumNormalizeHelperBase<ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase>
{
}

public class AlbumNormalizeModelHelper : AlbumNormalizeHelperBase<ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase>, IAlbumNormalizeHelper
{
    public AlbumNormalizeModelHelper(IRepository<ArtistMongoBase> artistsRepository, IRepository<Album> albumsRepository,
        IRepository<TrackMongoBase> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(
        artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}