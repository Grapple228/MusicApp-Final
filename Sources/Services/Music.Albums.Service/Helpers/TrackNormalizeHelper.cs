using Music.Albums.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Track;
using Music.Services.Database.MongoDb.Models;

namespace Music.Albums.Service.Helpers;

public interface ITrackNormalizeHelper : ITrackNormalizeHelperBase<ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase>
{
    
}

public class TrackNormalizeModelHelper : TrackNormalizeHelperBase<ArtistMongoBase, Album, TrackMongoBase, GenreMongoBase>, ITrackNormalizeHelper
{
    public TrackNormalizeModelHelper(IRepository<ArtistMongoBase> artistsRepository, IRepository<Album> albumsRepository, IRepository<TrackMongoBase> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}