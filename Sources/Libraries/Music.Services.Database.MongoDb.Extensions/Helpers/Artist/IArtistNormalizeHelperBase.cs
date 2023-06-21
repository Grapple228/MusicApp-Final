using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Artist;

public interface IArtistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
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
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModels(IEnumerable<IArtistMongo> models);
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModels(IArtistMongo model);

    Task<(
        TTrack[] Tracks,
        TAlbum[] Albums,
        TArtist[] Artists,
        TGenre[] Genres,
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModelsHelper(params Guid[] artistIds);
}