using Music.Artists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Track;
using Music.Services.Database.MongoDb.Models;

namespace Music.Artists.Service.Helpers;

public interface ITrackNormalizeHelper : ITrackNormalizeHelperBase<Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase>
{
    
}

public class TrackNormalizeModelHelper : TrackNormalizeHelperBase<Artist, AlbumMongoBase, TrackMongoBase, GenreMongoBase>, ITrackNormalizeHelper
{
    public TrackNormalizeModelHelper(IRepository<Artist> artistsRepository, IRepository<AlbumMongoBase> albumsRepository, IRepository<TrackMongoBase> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}