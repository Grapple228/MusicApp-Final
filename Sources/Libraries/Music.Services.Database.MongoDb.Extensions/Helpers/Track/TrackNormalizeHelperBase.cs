using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Track;

public abstract class TrackNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre> : 
    ITrackNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TArtist> _artistsRepository;
    private readonly IRepository<TAlbum> _albumsRepository;
    private readonly IRepository<TTrack> _tracksRepository;
    private readonly IRepository<TGenre> _genresRepository;

    protected TrackNormalizeHelperBase(
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
        IReadOnlyCollection<TArtist> ModelArtists, 
        IReadOnlyCollection<TAlbum> ModelAlbums,
        IReadOnlyCollection<TGenre> ModelGenres)> 
        GetModels(IReadOnlyCollection<ITrackMongo> models)
    {
        var modelAlbumIds = models.SelectMany(x => x.Albums).Distinct().ToArray();
        var modelArtistIds = models.SelectMany(x => x.Artists).Concat(models.Select(x => x.OwnerId)).Distinct().ToArray();
        var modelGenreIds = models.SelectMany(x => x.Genres).Distinct().ToArray();
        return await GetModelsHelper(modelArtistIds, modelAlbumIds, modelGenreIds);
    }

    public virtual async Task<(
            TTrack[] Tracks,
            TAlbum[] Albums,
            TArtist[] Artists,
            TGenre[] Genres,
            IReadOnlyCollection<TArtist> ModelArtists,
            IReadOnlyCollection<TAlbum> ModelAlbums,
            IReadOnlyCollection<TGenre> ModelGenres)>
        GetModels(ITrackMongo model)
    {
        var artists = new List<Guid>();
        artists.AddRange(model.Artists);
        if(!artists.Contains(model.OwnerId))
            artists.Add(model.OwnerId);
        
        return await GetModelsHelper(artists, model.Albums, model.Genres);
    }

    public virtual async Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TArtist> ModelArtists, 
        IReadOnlyCollection<TAlbum> ModelAlbums,
        IReadOnlyCollection<TGenre> ModelGenres)> 
        GetModelsHelper(IReadOnlyCollection<Guid> artistIds,
            IEnumerable<Guid> albumIds, IEnumerable<Guid> genreIds)
    {
        var modelArtists = await _artistsRepository.GetAllAsync(artistIds);
        var modelAlbums = await _albumsRepository.GetAllAsync(albumIds);

        var modelAlbumTrackIds = modelAlbums.SelectMany(x => x.Tracks).Distinct();
        var artistIdsToFind = modelAlbums.SelectMany(x => x.Artists).Concat(modelAlbums.Select(x => x.OwnerId)).Where(x => !artistIds.Contains(x)).Distinct();
        var allTracks = (await _tracksRepository.GetAllAsync(x => artistIds.Contains(x.OwnerId) || x.Artists.Any(a => artistIds.Contains(a)) 
                                                                 || modelAlbumTrackIds.Contains(x.Id))).ToArray();
        var allAlbums = modelAlbums
            .Union(await _albumsRepository.GetAllAsync(x => artistIds.Contains(x.OwnerId) || x.Artists.Any(a => artistIds.Contains(a)))).ToArray();
        var allArtists = modelArtists.Concat(await _artistsRepository.GetAllAsync(artistIdsToFind)).ToArray();
        
        var allGenres = (await _genresRepository.GetAllAsync(genreIds)).ToArray();
        
        return (allTracks, allAlbums, allArtists, allGenres, modelArtists, modelAlbums, allGenres);
    }
}