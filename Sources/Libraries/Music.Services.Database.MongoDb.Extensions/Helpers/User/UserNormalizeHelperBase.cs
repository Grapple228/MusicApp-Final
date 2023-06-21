using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.User;

public abstract class UserNormalizeHelperBase<TTrack, TUser, TPlaylist> : 
    IUserNormalizeHelperBase<TTrack, TUser, TPlaylist>
    where TTrack : ITrackMongo 
    where TUser : IUserMongo
    where TPlaylist : IPlaylistMongo
{
    private readonly IRepository<TTrack> _tracksRepository;
    private readonly IRepository<TUser> _usersRepository;
    private readonly IRepository<TPlaylist> _playlistsRepository;

    protected UserNormalizeHelperBase(
        IRepository<TTrack> tracksRepository, 
        IRepository<TUser> usersRepository,
        IRepository<TPlaylist> playlistsRepository)
    {
        _tracksRepository = tracksRepository;
        _usersRepository = usersRepository;
        _playlistsRepository = playlistsRepository;
    }
    
    
    public virtual async Task<(
            TTrack[] Tracks, 
            TUser[] Users, 
            IReadOnlyCollection<TPlaylist> ModelPlaylists)> 
        GetModels(IEnumerable<IUserMongo> models)
    {
        var playlistIds = models.SelectMany(x => x.Playlists).Distinct().ToArray();
        return await GetModelsHelper(playlistIds);
    }

    public virtual async Task<(
            TTrack[] Tracks, 
            TUser[] Users, 
            IReadOnlyCollection<TPlaylist> ModelPlaylists)> 
        GetModels(IUserMongo model)
    {
        return await GetModelsHelper(model.Playlists.ToArray());
    }

    public virtual async Task<(
        TTrack[] Tracks, 
        TUser[] Users, 
        IReadOnlyCollection<TPlaylist> ModelPlaylists)> 
        GetModelsHelper(IEnumerable<Guid> playlistIds)
    {
        var modelPlaylists = await _playlistsRepository.GetAllAsync(playlistIds);

        var allUserIds = modelPlaylists.Select(x => x.OwnerId).Distinct();
        var allUsers = (await _usersRepository.GetAllAsync(allUserIds)).ToArray();

        var allTrackIds = modelPlaylists.SelectMany(x => x.Tracks).Distinct();
        var allTracks = (await _tracksRepository.GetAllAsync(allTrackIds)).ToArray();

        return (allTracks, allUsers, modelPlaylists);
    }
}