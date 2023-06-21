using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Extensions.Helpers.Track;
using Music.Services.Database.MongoDb.Models;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Helpers;

public interface ITrackNormalizeHelper : ITrackNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, Track, GenreMongoBase>
{
}

public class TrackNormalizeHelper : TrackNormalizeHelperBase<ArtistMongoBase, AlbumMongoBase, Track, GenreMongoBase>, ITrackNormalizeHelper
{
    public TrackNormalizeHelper(IRepository<ArtistMongoBase> artistsRepository, IRepository<AlbumMongoBase> albumsRepository,
        IRepository<Track> tracksRepository, IRepository<GenreMongoBase> genresRepository) : base(
        artistsRepository, albumsRepository, tracksRepository, genresRepository)
    {
    }
}