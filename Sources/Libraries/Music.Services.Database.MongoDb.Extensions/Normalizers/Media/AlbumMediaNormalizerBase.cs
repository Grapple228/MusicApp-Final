using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Album;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Media.Models;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;

public interface IAlbumMediaNormalizer<in TAlbumInfo> : IMediaNormalizer<MediaAlbumDto, TAlbumInfo, IAlbumMongo> 
    where TAlbumInfo : IMediaModelBase, new()
{
}

public class AlbumMediaNormalizerBase<TNormalizer, TArtist, TAlbum, TTrack, TGenre, TAlbumInfo, TArtistInfo, TTrackInfo>
    : IAlbumMediaNormalizer<TAlbumInfo>
    where TNormalizer : IAlbumNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre>
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
    private readonly TNormalizer _albumNormalizeHelper;
    private readonly IRepository<TAlbum> _albumsRepository;

    public AlbumMediaNormalizerBase(IRepository<TAlbumInfo> albumInfosRepository,
        IRepository<TArtistInfo> artistInfosRepository, IRepository<TTrackInfo> trackInfosRepository, ServiceSettings serviceSettings,
        TNormalizer albumNormalizeHelper, IRepository<TAlbum> albumsRepository)
    {
        _albumInfosRepository = albumInfosRepository;
        _artistInfosRepository = artistInfosRepository;
        _trackInfosRepository = trackInfosRepository;
        _serviceSettings = serviceSettings;
        _albumNormalizeHelper = albumNormalizeHelper;
        _albumsRepository = albumsRepository;
    }

    protected async Task<(ICollection<TArtistInfo> ArtistInfos, ICollection<TTrackInfo> TrackInfos)> GetInfos(
        Guid userId, IEnumerable<ITrackMongo> modelTracks, IEnumerable<IArtistMongo> modelArtists)
    {
        var artistIds = modelArtists.Select(x => x.Id);
        var trackIds = modelTracks.Select(x => x.Id);
    
        var artistInfos =
            await _artistInfosRepository.GetAllAsync(x => x.UserId == userId && artistIds.Contains(x.MediaId));
        var trackInfos = 
            await _trackInfosRepository.GetAllAsync(x => x.UserId == userId && trackIds.Contains(x.MediaId));

        var allArtistInfos = new List<TArtistInfo>(artistInfos);
        var allTrackInfos = new List<TTrackInfo>(trackInfos);
        var notFoundArtists = artistIds.Where(x => artistInfos.All(a => a.MediaId != x))
            .Select(x => MediaCreator<TArtistInfo>.Create(x, userId));
        var notFoundTracks = trackIds.Where(x => trackInfos.All(t => t.MediaId != x))
            .Select(x => MediaCreator<TTrackInfo>.Create(x, userId));
        allArtistInfos.AddRange(notFoundArtists);
        allTrackInfos.AddRange(notFoundTracks);
        return (allArtistInfos, allTrackInfos);
    }
    
    public virtual async Task<MediaAlbumDto> Normalize(IAlbumMongo album, Guid userId)
    {
        var allModels = await _albumNormalizeHelper.GetModels(album);
        var normalizedAlbum = album.Normalize(allModels, _serviceSettings);
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks, (IEnumerable<IArtistMongo>)allModels.ModelArtists);
        
        var albumInfo = await _albumInfosRepository.GetAsync(x => x.UserId == userId && x.MediaId == album.Id)
                        ?? MediaCreator<TAlbumInfo>.Create(album.Id, userId);
        var artists = normalizedAlbum.Artists.Select(x => x.AsShortDto(infos.ArtistInfos.First(a => a.MediaId == x.Id)));
        var tracks = normalizedAlbum.Tracks.Select(x => x.AsShortDto(infos.TrackInfos.First(t => t.MediaId == x.Id)));
        return albumInfo.AsDto(normalizedAlbum, artists, tracks);
    }
    
    public virtual async Task<IReadOnlyCollection<MediaAlbumDto>> Normalize(IReadOnlyCollection<IAlbumMongo> albums, Guid userId)
    {
        var albumIds = albums.Select(x => x.Id);
        var albumInfos = await _albumInfosRepository.GetAllAsync(x => x.UserId == userId && albumIds.Contains(x.MediaId));
        return await NormalizeHelper(userId, albums, albumInfos);
    }

    public virtual async Task<IReadOnlyCollection<MediaAlbumDto>> Normalize(IReadOnlyCollection<TAlbumInfo> albumInfos, Guid userId)
    {
        var albumIds = albumInfos.Select(x => x.MediaId);
        var albums = await _albumsRepository.GetAllAsync(albumIds);
        return await NormalizeHelper(userId, (IReadOnlyCollection<IAlbumMongo>)albums, albumInfos);
    }
    
    private async Task<IReadOnlyCollection<MediaAlbumDto>> NormalizeHelper(Guid userId, IReadOnlyCollection<IAlbumMongo> albums,
        IReadOnlyCollection<TAlbumInfo> albumInfos)
    {
        var allModels = await _albumNormalizeHelper.GetModels(albums);
        var normalizedAlbums = albums.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks, (IEnumerable<IArtistMongo>)allModels.ModelArtists);
        
        return (from album in normalizedAlbums
            let albumInfo = albumInfos.FirstOrDefault(x => x.MediaId == album.Id) ?? MediaCreator<TAlbumInfo>.Create(album.Id, userId)
            let artists = album.Artists.Select(x => x.AsShortDto(infos.ArtistInfos.First(a => a.MediaId == x.Id)))
            let tracks = album.Tracks.Select(x => x.AsShortDto(infos.TrackInfos.First(t => t.MediaId == x.Id)))
            select albumInfo.AsDto(album, artists, tracks)).ToList();
    }
}