using Music.Artists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Extensions.Helpers.Artist;
using Music.Services.Database.MongoDb.Models;

namespace Music.Artists.Service.Helpers;

public interface IArtistNormalizeHelper : IArtistNormalizeHelperBase<Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase>
{
}

public class ArtistNormalizeHelper : ArtistNormalizeHelperBase<Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase>, IArtistNormalizeHelper
{
    public ArtistNormalizeHelper(IRepository<Artist> artistsRepository, IRepository<AlbumMongoBase> albumsRepository,
        IRepository<TrackMongoBase> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(
        artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}