using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Track;
using Music.Services.Database.MongoDb.Models;

namespace Music.Playlists.Service.Helpers;

public interface ITrackNormalizeHelper : ITrackNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase>
{
    
}

public class TrackNormalizeModelHelper : TrackNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, TrackMongoBase, GenreMongoBase>, ITrackNormalizeHelper
{
    public TrackNormalizeModelHelper(IRepository<ArtistMongoBase> artistsRepository, IRepository<AlbumMongoBase> albumsRepository, IRepository<TrackMongoBase> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}