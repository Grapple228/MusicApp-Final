using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Artist;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;

public interface IArtistMediaNormalizer<in TArtistInfo> : IMediaNormalizer<MediaArtistDto, TArtistInfo, IArtistMongo> 
    where TArtistInfo : IMediaModelBase, new()
{
}

public class ArtistMediaNormalizerBase<TNormalizer, TArtist, TAlbum, TTrack, TGenre, TAlbumInfo, TArtistInfo, TTrackInfo>
    : IArtistMediaNormalizer<TArtistInfo>
    where TNormalizer : IArtistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
    where TAlbumInfo : IMediaModelBase, new()
    where TArtistInfo : IMediaModelBase, new()
    where TTrackInfo : IMediaModelBase, new()
    where TArtist : IArtistMongo
    where TTrack : ITrackMongo
    where TGenre : IGenreMongo
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TAlbumInfo> _albumInfosRepository;
    private readonly IRepository<TArtistInfo> _artistInfosRepository;
    private readonly IRepository<TTrackInfo> _trackInfosRepository;
    private readonly ServiceSettings _serviceSettings;
    private readonly TNormalizer _artistNormalizeHelper;
    private readonly IRepository<TArtist> _artistsRepository;

    public ArtistMediaNormalizerBase(IRepository<TAlbumInfo> albumInfosRepository,
        IRepository<TArtistInfo> artistInfosRepository, IRepository<TTrackInfo> trackInfosRepository, 
        ServiceSettings serviceSettings, TNormalizer artistNormalizeHelper, IRepository<TArtist> artistsRepository)
    {
        _albumInfosRepository = albumInfosRepository;
        _artistInfosRepository = artistInfosRepository;
        _trackInfosRepository = trackInfosRepository;
        _serviceSettings = serviceSettings;
        _artistNormalizeHelper = artistNormalizeHelper;
        _artistsRepository = artistsRepository;
    }

    protected async Task<(ICollection<TAlbumInfo> AlbumInfos, ICollection<TTrackInfo> TrackInfos)> GetInfos(
        Guid userId, IEnumerable<ITrackMongo> modelTracks, IEnumerable<IAlbumMongo> modelAlbums)
    {
        var albumIds = modelAlbums.Select(x => x.Id);
        var trackIds = modelTracks.Select(x => x.Id);
    
        var albumInfos =
            await _albumInfosRepository.GetAllAsync(x => x.UserId == userId && albumIds.Contains(x.MediaId));
        var trackInfos = 
            await _trackInfosRepository.GetAllAsync(x => x.UserId == userId && trackIds.Contains(x.MediaId));

        var allAlbumInfos = new List<TAlbumInfo>(albumInfos);
        var allTrackInfos = new List<TTrackInfo>(trackInfos);
        var notFoundAlbums = albumIds.Where(x => albumInfos.All(a => a.MediaId != x))
            .Select(x => MediaCreator<TAlbumInfo>.Create(x, userId));
        var notFoundTracks = trackIds.Where(x => trackInfos.All(t => t.MediaId != x))
            .Select(x => MediaCreator<TTrackInfo>.Create(x, userId));
        allAlbumInfos.AddRange(notFoundAlbums);
        allTrackInfos.AddRange(notFoundTracks);
        return (allAlbumInfos, allTrackInfos);
    }

    public async Task<MediaArtistDto> Normalize(IArtistMongo artist, Guid userId)
    {
        var allModels = await _artistNormalizeHelper.GetModels(artist);
        var normalizedArtist = artist.Normalize(allModels, _serviceSettings);
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks, (IEnumerable<IAlbumMongo>)allModels.ModelAlbums);
        
        var artistInfo = await _artistInfosRepository.GetAsync(x => x.UserId == userId && x.MediaId == artist.Id)
                        ?? MediaCreator<TArtistInfo>.Create(artist.Id, userId);
        var albums = normalizedArtist.Albums.Select(x => x.AsShortDto(infos.AlbumInfos.First(a => a.MediaId == x.Id)));
        var tracks = normalizedArtist.Tracks.Select(x => x.AsShortDto(infos.TrackInfos.First(t => t.MediaId == x.Id)));
        return artistInfo.AsDto(normalizedArtist, albums, tracks);
    }

    public async Task<IReadOnlyCollection<MediaArtistDto>> Normalize(IReadOnlyCollection<TArtistInfo> artistInfos, Guid userId)
    {
        var artistIds = artistInfos.Select(x => x.MediaId);
        var artists = await _artistsRepository.GetAllAsync(artistIds);
        return await NormalizeHelper(userId, (IReadOnlyCollection<IArtistMongo>)artists, artistInfos);
    }

    public async Task<IReadOnlyCollection<MediaArtistDto>> Normalize(IReadOnlyCollection<IArtistMongo> artists, Guid userId)
    {
        var artistIds = artists.Select(x => x.Id);
        var artistInfos = await _artistInfosRepository.GetAllAsync(x => x.UserId == userId && artistIds.Contains(x.MediaId));
        return await NormalizeHelper(userId, artists, artistInfos);
    }
    
    private async Task<IReadOnlyCollection<MediaArtistDto>> NormalizeHelper(Guid userId, IReadOnlyCollection<IArtistMongo> artists,
        IReadOnlyCollection<TArtistInfo> artistInfos)
    {
        var allModels = await _artistNormalizeHelper.GetModels(artists);
        var normalizedArtists = artists.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks, (IEnumerable<IAlbumMongo>)allModels.ModelAlbums);
        
        return (from artist in normalizedArtists
            let artistInfo = artistInfos.FirstOrDefault(x => x.MediaId == artist.Id) ?? MediaCreator<TArtistInfo>.Create(artist.Id, userId)
            let albums = artist.Albums.Select(x => x.AsShortDto(infos.AlbumInfos.First(a => a.MediaId == x.Id)))
            let tracks = artist.Tracks.Select(x => x.AsShortDto(infos.TrackInfos.First(t => t.MediaId == x.Id)))
            select artistInfo.AsDto(artist, albums, tracks)).ToList();
    }
}