using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.User;

public interface IUserNormalizeHelperBase<TTrack, TUser, TPlaylist>
    where TTrack : ITrackMongo 
    where TUser : IUserMongo
    where TPlaylist : IPlaylistMongo
{
    Task<(
        TTrack[] Tracks, 
        TUser[] Users, 
        IReadOnlyCollection<TPlaylist> ModelPlaylists)> GetModels(IEnumerable<IUserMongo> models);
    Task<(
        TTrack[] Tracks, 
        TUser[] Users, 
        IReadOnlyCollection<TPlaylist> ModelPlaylists)> GetModels(IUserMongo model);

    Task<(
        TTrack[] Tracks, 
        TUser[] Users, 
        IReadOnlyCollection<TPlaylist> ModelPlaylists)> GetModelsHelper(
        IEnumerable<Guid> playlistIds);
}