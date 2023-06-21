using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Album;

public interface IAlbumNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
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
        IReadOnlyCollection<TArtist> ModelArtists)> GetModels(IReadOnlyCollection<IAlbumMongo> models);
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TArtist> ModelArtists)> GetModels(IAlbumMongo model);

    Task<(
        TTrack[] Tracks,
        TAlbum[] Albums,
        TArtist[] Artists,
        TGenre[] Genres,
        IReadOnlyCollection<TTrack> ModelTracks,
        IReadOnlyCollection<TArtist> ModelArtists)> GetModelsHelper(IReadOnlyCollection<Guid> artistIds,
        IEnumerable<Guid> trackIds);
}