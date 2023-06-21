using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Playlist;

public interface IPlaylistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre, TUser>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
    where TUser : IUserMongo
{
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TUser> ModelUsers)> GetModels(IReadOnlyCollection<IPlaylistMongo> models);
    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TUser> ModelUsers)> GetModels(IPlaylistMongo model);

    Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks,
        IReadOnlyCollection<TUser> ModelUsers)> GetModelsHelper(IEnumerable<Guid> trackIds, params Guid[] userIds);
}