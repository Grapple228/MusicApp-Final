using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Track;

public interface ITrackNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
{
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TArtist> ModelArtists, 
        IReadOnlyCollection<TAlbum> ModelAlbums,
        IReadOnlyCollection<TGenre> ModelGenres)> GetModels(IReadOnlyCollection<ITrackMongo> models);
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TArtist> ModelArtists, 
        IReadOnlyCollection<TAlbum> ModelAlbums,
        IReadOnlyCollection<TGenre> ModelGenres)> GetModels(ITrackMongo model);

    Task<(
        TTrack[] Tracks,
        TAlbum[] Albums,
        TArtist[] Artists,
        TGenre[] Genres,
        IReadOnlyCollection<TArtist> ModelArtists, 
        IReadOnlyCollection<TAlbum> ModelAlbums,
        IReadOnlyCollection<TGenre> ModelGenres)> GetModelsHelper(IReadOnlyCollection<Guid> artistIds,
        IEnumerable<Guid> albumIds, IEnumerable<Guid> genreIds);
}