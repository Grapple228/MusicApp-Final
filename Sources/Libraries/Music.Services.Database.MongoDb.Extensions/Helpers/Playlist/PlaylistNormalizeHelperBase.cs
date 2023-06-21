using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Playlist;

public class PlaylistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre, TUser> : 
    IPlaylistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre, TUser>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
    where TUser : IUserMongo
{
    private readonly IRepository<TArtist> _artistsRepository;
    private readonly IRepository<TAlbum> _albumsRepository;
    private readonly IRepository<TTrack> _tracksRepository;
    private readonly IRepository<TGenre> _genresRepository;
    private readonly IRepository<TUser> _usersRepository;

    public PlaylistNormalizeHelperBase(IRepository<TArtist> artistsRepository, IRepository<TAlbum> albumsRepository,
        IRepository<TTrack> tracksRepository, IRepository<TGenre> genresRepository, IRepository<TUser> usersRepository)
    {
        _artistsRepository = artistsRepository;
        _albumsRepository = albumsRepository;
        _tracksRepository = tracksRepository;
        _genresRepository = genresRepository;
        _usersRepository = usersRepository;
    }
    
    public virtual async Task<(TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack>
        ModelTracks, IReadOnlyCollection<TUser> ModelUsers)> GetModels(IReadOnlyCollection<IPlaylistMongo> models)
    {
        var modelTrackIds = models.SelectMany(x => x.Tracks).Distinct().ToArray();
        var modelUserIds = models.Select(x => x.OwnerId).Distinct().ToArray();
        return await GetModelsHelper(modelTrackIds, modelUserIds);
    }

    public virtual async Task<(TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack>
        ModelTracks, IReadOnlyCollection<TUser> ModelUsers)> GetModels(IPlaylistMongo model) =>
        await GetModelsHelper(model.Tracks.ToArray(), model.OwnerId);

    public virtual async Task<(TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack>
        ModelTracks, IReadOnlyCollection<TUser> ModelUsers)> GetModelsHelper(IEnumerable<Guid> trackIds, params Guid[] userIds)
    {
        var modelUsers = await _usersRepository.GetAllAsync(userIds);
        var allTracks = (await _tracksRepository.GetAllAsync(trackIds)).ToArray();

        var trackArtistIds = allTracks.SelectMany(x => x.Artists).Concat(allTracks.Select(x => x.OwnerId)).Distinct();
        var allArtists = (await _artistsRepository.GetAllAsync(trackArtistIds)).ToArray();
        var allArtistIds = allArtists.Select(x => x.Id);
        var allTrackAlbumIds = allTracks.SelectMany(x => x.Albums).Distinct();
        
        var allAlbums = (await _albumsRepository.GetAllAsync(a =>
            a.Artists.Any(x => allArtistIds.Contains(x)) || allTrackAlbumIds.Contains(a.Id))).ToArray();
        
        var allGenreIds = allTracks.SelectMany(x => x.Genres);
        var allGenres = (await _genresRepository.GetAllAsync(allGenreIds)).ToArray();

        return (allTracks, allAlbums, allArtists, allGenres, allTracks, modelUsers);
    }
}