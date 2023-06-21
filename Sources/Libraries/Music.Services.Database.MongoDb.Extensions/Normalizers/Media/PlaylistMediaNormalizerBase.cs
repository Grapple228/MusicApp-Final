using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Helpers.Playlist;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Media.Models;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;


public interface IPlaylistMediaNormalizer<in TPlaylistInfo> : IMediaNormalizer<MediaPlaylistDto, TPlaylistInfo, IPlaylistMongo> 
    where TPlaylistInfo : IMediaModelBase, new()
{
}

public class PlaylistMediaNormalizerBase<TNormalizer, TArtist, TAlbum, TTrack, TGenre, TUser, TPlaylist, TPlaylistInfo, TTrackInfo>
    : IPlaylistMediaNormalizer<TPlaylistInfo>
    where TNormalizer : IPlaylistNormalizeHelperBase<TArtist, TAlbum, TTrack, TGenre, TUser>
    where TTrackInfo : IMediaModelBase, new()
    where TPlaylistInfo : IMediaModelBase, new()
    where TPlaylist : IPlaylistMongo
    where TArtist : IArtistMongo
    where TTrack : ITrackMongo
    where TGenre : IGenreMongo
    where TAlbum : IAlbumMongo
    where TUser : IUserMongo
{
    private readonly IRepository<TTrackInfo> _trackInfosRepository;
    private readonly IRepository<TPlaylistInfo> _playlistInfosRepository;
    private readonly TNormalizer _playlistNormalizeHelper;
    private readonly ServiceSettings _serviceSettings;
    private readonly IRepository<TPlaylist> _playlistsRepository;

    public PlaylistMediaNormalizerBase(IRepository<TTrackInfo> trackInfosRepository, IRepository<TPlaylistInfo> playlistInfosRepository, TNormalizer playlistNormalizeHelper,
        ServiceSettings serviceSettings, IRepository<TPlaylist> playlistsRepository)
    {
        _trackInfosRepository = trackInfosRepository;
        _playlistInfosRepository = playlistInfosRepository;
        _playlistNormalizeHelper = playlistNormalizeHelper;
        _serviceSettings = serviceSettings;
        _playlistsRepository = playlistsRepository;
    }
    
    protected async Task<ICollection<TTrackInfo>> GetInfos(Guid userId, IEnumerable<ITrackMongo> modelTracks)
    {
        var trackIds = modelTracks.Select(x => x.Id);

        var trackInfos = await _trackInfosRepository.GetAllAsync(x => x.UserId == userId && trackIds.Contains(x.MediaId));

        var allTrackInfos = new List<TTrackInfo>(trackInfos);
        var notFoundTracks = trackIds.Where(x => trackInfos.All(t => t.MediaId != x))
            .Select(x => MediaCreator<TTrackInfo>.Create(x, userId));
        allTrackInfos.AddRange(notFoundTracks);
        
        return allTrackInfos;
    }
    
    public async Task<MediaPlaylistDto> Normalize(IPlaylistMongo playlist, Guid userId)
    {
        var allModels = await _playlistNormalizeHelper.GetModels(playlist);
        var normalizedPlaylist = playlist.Normalize(allModels, _serviceSettings);
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks);
        
        var playlistInfo = await _playlistInfosRepository.GetAsync(x => x.UserId == userId && x.MediaId == playlist.Id)
                        ?? MediaCreator<TPlaylistInfo>.Create(playlist.Id, userId);
        var tracks = normalizedPlaylist.Tracks.Select(x => x.AsShortDto(infos.First(a => a.MediaId == x.Id)));
        return playlistInfo.AsDto(normalizedPlaylist, tracks);
    }

    public async Task<IReadOnlyCollection<MediaPlaylistDto>> Normalize(IReadOnlyCollection<TPlaylistInfo> playlistInfos, Guid userId)
    {
        var playlistIds = playlistInfos.Select(x => x.MediaId);
        var playlists = await _playlistsRepository.GetAllAsync(playlistIds);
        return await NormalizeHelper(userId, (IReadOnlyCollection<IPlaylistMongo>)playlists, playlistInfos);
    }

    public async Task<IReadOnlyCollection<MediaPlaylistDto>> Normalize(IReadOnlyCollection<IPlaylistMongo> playlists, Guid userId)
    {
        var playlistIds = playlists.Select(x => x.Id);
        var playlistInfos = await _playlistInfosRepository.GetAllAsync(x => x.UserId == userId && playlistIds.Contains(x.MediaId));
        return await NormalizeHelper(userId, playlists, playlistInfos);
    }
    
    private async Task<IReadOnlyCollection<MediaPlaylistDto>> NormalizeHelper(Guid userId, IReadOnlyCollection<IPlaylistMongo> playlists,
        IReadOnlyCollection<TPlaylistInfo> playlistInfos)
    {
        var allModels = await _playlistNormalizeHelper.GetModels(playlists);
        var normalizedPlaylists = playlists.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
        var infos = await GetInfos(userId, (IEnumerable<ITrackMongo>)allModels.ModelTracks);
        
        return (from playlist in normalizedPlaylists
            let playlistInfo = playlistInfos.FirstOrDefault(x => x.MediaId == playlist.Id) ?? MediaCreator<TPlaylistInfo>.Create(playlist.Id, userId)
            let tracks = playlist.Tracks.Select(x => x.AsShortDto(infos.First(i => i.MediaId == x.Id)))
            select playlistInfo.AsDto(playlist, tracks)).ToList();
    }
}