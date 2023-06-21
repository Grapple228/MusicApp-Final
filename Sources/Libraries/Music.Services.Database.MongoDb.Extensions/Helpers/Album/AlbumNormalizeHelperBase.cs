using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;

namespace Music.Services.Database.MongoDb.Extensions.Helpers.Album;

public abstract class AlbumNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre> : 
    IAlbumNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
    where TArtist : IArtistMongo 
    where TTrack : ITrackMongo 
    where TGenre : IGenreMongo 
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TArtist> _artistsRepository;
    private readonly IRepository<TAlbum> _albumsRepository;
    private readonly IRepository<TTrack> _tracksRepository;
    private readonly IRepository<TGenre> _genresRepository;

    protected AlbumNormalizeHelperBase(
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
        IReadOnlyCollection<TArtist> ModelArtists)> GetModels(IReadOnlyCollection<IAlbumMongo> models)
    {
        var modelTrackIds = models.SelectMany(x => x.Tracks).Distinct().ToArray();
        var modelOwners = models.Select(x => x.OwnerId);
        var modelArtistIds = models.SelectMany(x => x.Artists).Concat(modelOwners).Distinct().ToArray();
        return await GetModelsHelper(modelArtistIds, modelTrackIds);
    }

    public virtual async Task<(
        TTrack[] Tracks,
        TAlbum[] Albums,
        TArtist[] Artists,
        TGenre[] Genres,
        IReadOnlyCollection<TTrack> ModelTracks,
        IReadOnlyCollection<TArtist> ModelArtists)> GetModels(IAlbumMongo model)
    {
        var artistIds = model.Artists.ToList();
        artistIds.Add(model.OwnerId);
        return await GetModelsHelper(artistIds, model.Tracks);
    }
        
    
    public virtual async Task<(
        TTrack[] Tracks, 
        TAlbum[] Albums, 
        TArtist[] Artists, 
        TGenre[] Genres, 
        IReadOnlyCollection<TTrack> ModelTracks, 
        IReadOnlyCollection<TArtist> ModelArtists)> GetModelsHelper(IReadOnlyCollection<Guid> artistIds, IEnumerable<Guid> trackIds)
    {
        var modelTracks = await _tracksRepository.GetAllAsync(trackIds);
        var modelArtists = await _artistsRepository.GetAllAsync(artistIds);

        var allTracks = modelTracks
            .Union(await _tracksRepository.GetAllAsync(t => artistIds.Contains(t.OwnerId) || t.Artists.Any(a => artistIds.Contains(a)))).ToArray();
        var allArtists = modelArtists
            .Concat(await _artistsRepository.GetAllAsync(modelTracks.SelectMany(x => x.Artists)
                .Concat(modelTracks.Select(x => x.OwnerId)).Where(x => !artistIds.Contains(x)))).ToArray();
        
        var allArtistIds = allArtists.Select(k => k.Id);
        var allTrackAlbumIds = allTracks.SelectMany(x => x.Albums).Distinct();
        var allAlbums = (await _albumsRepository.GetAllAsync(a =>
            a.Artists.Any(x => allArtistIds.Contains(x)) || allTrackAlbumIds.Contains(a.Id))).ToArray();
        var allGenreIds = allTracks.SelectMany(x => x.Genres).Distinct();
        var allGenres = (await _genresRepository.GetAllAsync(allGenreIds)).ToArray();
        
        return (Tracks: allTracks, 
            Albums: allAlbums,
            Artists: allArtists, 
            Genres: allGenres, 
            ModelTracks: modelTracks, 
            ModelArtists: modelArtists);
    }
}