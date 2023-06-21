using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Track;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;

public interface ITrackMediaNormalizer<in TTrackInfo> : IMediaNormalizer<MediaTrackDto, TTrackInfo, ITrackMongo> 
    where TTrackInfo : IMediaModelBase, new()
{
}

public class TrackMediaNormalizerBase<TNormalizer, TArtist, TAlbum, TTrack, TGenre, TAlbumInfo, TArtistInfo, TTrackInfo>
    : ITrackMediaNormalizer<TTrackInfo>
    where TNormalizer : ITrackNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
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
    private readonly TNormalizer _trackNormalizeHelper;
    private readonly IRepository<TTrack> _tracksRepository;

    public TrackMediaNormalizerBase(IRepository<TAlbumInfo> albumInfosRepository,
        IRepository<TArtistInfo> artistInfosRepository, IRepository<TTrackInfo> trackInfosRepository, 
        ServiceSettings serviceSettings, TNormalizer trackNormalizeHelper, IRepository<TTrack> tracksRepository)
    {
        _albumInfosRepository = albumInfosRepository;
        _artistInfosRepository = artistInfosRepository;
        _trackInfosRepository = trackInfosRepository;
        _serviceSettings = serviceSettings;
        _trackNormalizeHelper = trackNormalizeHelper;
        _tracksRepository = tracksRepository;
    }

    protected async Task<(ICollection<TAlbumInfo> AlbumInfos, ICollection<TArtistInfo> ArtistInfos)> GetInfos(
        Guid userId, IEnumerable<IArtistMongo> modelArtists, IEnumerable<IAlbumMongo> modelAlbums)
    {
        var albumIds = modelAlbums.Select(x => x.Id);
        var artistIds = modelArtists.Select(x => x.Id);
    
        var albumInfos =
            await _albumInfosRepository.GetAllAsync(x => x.UserId == userId && albumIds.Contains(x.MediaId));
        var artistInfos = 
            await _artistInfosRepository.GetAllAsync(x => x.UserId == userId && artistIds.Contains(x.MediaId));

        var allAlbumInfos = new List<TAlbumInfo>(albumInfos);
        var allArtistInfos = new List<TArtistInfo>(artistInfos);
        var notFoundAlbums = albumIds.Where(x => albumInfos.All(a => a.MediaId != x))
            .Select(x => MediaCreator<TAlbumInfo>.Create(x, userId));
        var notFoundArtists = artistIds.Where(x => artistInfos.All(t => t.MediaId != x))
            .Select(x => MediaCreator<TArtistInfo>.Create(x, userId));
        allAlbumInfos.AddRange(notFoundAlbums);
        allArtistInfos.AddRange(notFoundArtists);
        return (allAlbumInfos, allArtistInfos);
    }

    public async Task<MediaTrackDto> Normalize(ITrackMongo track, Guid userId)
    {
        var allModels = await _trackNormalizeHelper.GetModels(track);
        var normalizedTrack = track.Normalize(allModels, _serviceSettings);
        var infos = await GetInfos(userId, (IEnumerable<IArtistMongo>)allModels.ModelArtists, (IEnumerable<IAlbumMongo>)allModels.ModelAlbums);
        
        var trackInfo = await _trackInfosRepository.GetAsync(x => x.UserId == userId && x.MediaId == track.Id)
                         ?? MediaCreator<TTrackInfo>.Create(track.Id, userId);
        var albums = normalizedTrack.Albums.Select(x => x.AsShortDto(infos.AlbumInfos.First(a => a.MediaId == x.Id)));
        var artists = normalizedTrack.Artists.Select(x => x.AsShortDto(infos.ArtistInfos.First(t => t.MediaId == x.Id)));
        return trackInfo.AsDto(normalizedTrack, artists, albums);
    }

    public async Task<IReadOnlyCollection<MediaTrackDto>> Normalize(IReadOnlyCollection<TTrackInfo> trackInfos, Guid userId)
    {
        var trackIds = trackInfos.Select(x => x.MediaId);
        var tracks = await _tracksRepository.GetAllAsync(trackIds);
        return await NormalizeHelper(userId, (IReadOnlyCollection<ITrackMongo>)tracks, trackInfos);
    }

    public async Task<IReadOnlyCollection<MediaTrackDto>> Normalize(IReadOnlyCollection<ITrackMongo> tracks, Guid userId)
    {
        var trackIds = tracks.Select(x => x.Id);
        var trackInfos = await _trackInfosRepository.GetAllAsync(x => x.UserId == userId && trackIds.Contains(x.MediaId));
        return await NormalizeHelper(userId, tracks, trackInfos);
    }

    private async Task<IReadOnlyCollection<MediaTrackDto>> NormalizeHelper(Guid userId, IReadOnlyCollection<ITrackMongo> tracks,
        IReadOnlyCollection<TTrackInfo> trackInfos)
    {
        var allModels = await _trackNormalizeHelper.GetModels(tracks);
        var normalizedTracks = tracks.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
        var infos = await GetInfos(userId, (IEnumerable<IArtistMongo>)allModels.ModelArtists, (IEnumerable<IAlbumMongo>)allModels.ModelAlbums);
        
        return (from track in normalizedTracks
            let trackInfo = trackInfos.FirstOrDefault(x => x.MediaId == track.Id) ?? MediaCreator<TTrackInfo>.Create(track.Id, userId)
            let albums = track.Albums.Select(x => x.AsShortDto(infos.AlbumInfos.First(a => a.MediaId == x.Id)))
            let artists = track.Artists.Select(x => x.AsShortDto(infos.ArtistInfos.First(t => t.MediaId == x.Id)))
            select trackInfo.AsDto(track, artists, albums)).ToList();
    }
}