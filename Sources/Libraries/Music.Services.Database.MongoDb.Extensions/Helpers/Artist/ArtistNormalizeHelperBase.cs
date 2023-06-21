using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Artist;

public abstract class ArtistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre> : 
    IArtistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TArtist> _artistsRepository;
    private readonly IRepository<TAlbum> _albumsRepository;
    private readonly IRepository<TTrack> _tracksRepository;
    private readonly IRepository<TGenre> _genresRepository;

    protected ArtistNormalizeHelperBase(
        IRepository<TArtist> artistsRepository, 
        IRepository<TAlbum> albumsRepository,
        IRepository<TTrack> tracksRepository, 
        IRepository<TGenre> genresRepository)
    {
        _artistsRepository = artistsRepository;
        _albumsRepository = albumsRepository;
        _tracksRepository = tracksRepository;
        _genresRepository = genresRepository;
    }
    
    public virtual async Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModels(IEnumerable<IArtistMongo> models)
    {
        var artistIds = models.Select(x => x.Id).ToArray();
        return await GetModelsHelper(artistIds);
    }

    public virtual async Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModels(IArtistMongo model) =>
        await GetModelsHelper(model.Id);
    
    public virtual async Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TAlbum> ModelAlbums)> GetModelsHelper(params Guid[] artistIds)
    {
        var modelAlbums = await _albumsRepository.GetAllAsync(x => artistIds.Contains(x.OwnerId) || x.Artists.Any(a => artistIds.Contains(a)));
        var modelTracks = await _tracksRepository.GetAllAsync(x => artistIds.Contains(x.OwnerId) || x.Artists.Any(a => artistIds.Contains(a)));
        var modelTrackIds = modelTracks.Select(x => x.Id);
        var modelAlbumIds = modelAlbums.Select(x => x.Id);

        var modelAlbumTrackIds =
            modelAlbums.SelectMany(x => x.Tracks).Where(x => !modelTrackIds.Contains(x)).Distinct();
        var modelTrackAlbumIds =
            modelTracks.SelectMany(x => x.Albums).Where(x => !modelAlbumIds.Contains(x)).Distinct();
        
        var allArtistIds = artistIds.Concat(modelAlbums.SelectMany(x => x.Artists).Concat(modelAlbums.Select(x => x.OwnerId)))
            .Concat(modelTracks.SelectMany(x => x.Artists).Concat(modelTracks.Select(x => x.OwnerId))).Distinct();
        var allTracks = modelTracks.Concat(await _tracksRepository.GetAllAsync(t => artistIds.Contains(t.OwnerId) || modelAlbumTrackIds.Contains(t.Id))).Distinct()
            .ToArray();
        var allAlbums = modelAlbums.Concat(await _albumsRepository.GetAllAsync(a => artistIds.Contains(a.OwnerId) || modelTrackAlbumIds.Contains(a.Id))).Distinct()
            .ToArray();
        var allArtists = (await _artistsRepository.GetAllAsync(allArtistIds)).ToArray();
        var allGenreIds = modelTracks.SelectMany(x => x.Genres).Distinct();
        var allGenres = (await _genresRepository.GetAllAsync(allGenreIds)).ToArray();
        
        return (Tracks: allTracks, 
            Albums: allAlbums,
            Artists: allArtists, 
            Genres: allGenres, 
            ModelTracks: modelTracks, 
            ModelAlbums: modelAlbums);
    }
}