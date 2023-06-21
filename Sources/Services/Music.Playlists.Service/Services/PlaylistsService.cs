using MassTransit;
using MongoDB.Driver.Linq;
using Music.Playlists.Service.Helpers;
using Music.Playlists.Service.Models;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Normalizers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Identity.Common;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.Common;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Requests.Playlists;

namespace Music.Playlists.Service.Services;

public interface IPlaylistsService
{
    Task<PageResult<PlaylistDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null);
    Task<IReadOnlyCollection<PlaylistDto>> GetAll(long count = long.MaxValue, string? searchQuery = null);
    Task<IReadOnlyCollection<PlaylistDto>> GetUserPlaylists(Guid userId);
    Task<PlaylistDto> Get(Guid playlistId);
    Task<PlaylistDto> Create(PlaylistCreateRequest request, Guid userId, HttpContext context);
    Task ChangePlaylistTitle(Guid playlistId, string title, Guid userId, HttpContext context);
    Task ChangePlaylistPrivacy(Guid playlistId, bool isPublic, Guid userId, HttpContext context);
    Task Delete(Guid playlistId, Guid userId, HttpContext context);
    Task AddTrackToPlaylist(Guid playlistId, Guid trackId, Guid userId, HttpContext context);
    Task RemoveTrackFromPlaylist(Guid playlistId, Guid trackId, Guid userId, HttpContext context);
    Task AddTracksFromArtist(Guid playlistId, Guid artistToAddId, Guid userId, HttpContext context);
    Task AddTracksFromAlbum(Guid playlistId, Guid albumToAddId, Guid userId, HttpContext context);
    Task AddTracksFromPlaylist(Guid playlistId, Guid playlistToAddId, Guid userId, HttpContext context);
}

public class PlaylistsService : IPlaylistsService
{
    private readonly IRepository<Playlist> _playlistsRepository;
    private readonly IRepository<UserMongoBase> _usersRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ServiceSettings _serviceSettings;
    private readonly IPlaylistNormalizeHelper _playlistNormalizeHelper;
    private readonly IRepository<TrackMongoBase> _tracksRepository;
    private readonly IRepository<ArtistMongoBase> _artistsRepository;
    private readonly IRepository<AlbumMongoBase> _albumsRepository;

    public PlaylistsService(IRepository<Playlist> playlistsRepository, IRepository<UserMongoBase> usersRepository,
        IPublishEndpoint publishEndpoint, ServiceSettings serviceSettings, IPlaylistNormalizeHelper playlistNormalizeHelper,
        IRepository<TrackMongoBase> tracksRepository, IRepository<ArtistMongoBase> artistsRepository, IRepository<AlbumMongoBase> albumsRepository)

    {
        _playlistsRepository = playlistsRepository;
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
        _serviceSettings = serviceSettings;
        _playlistNormalizeHelper = playlistNormalizeHelper;
        _tracksRepository = tracksRepository;
        _artistsRepository = artistsRepository;
        _albumsRepository = albumsRepository;
    }

    public async Task<PageResult<PlaylistDto>> GetPage(long pageNumber, long countPerPage = 30, string? searchQuery = null)
   {
       var pageResult = searchQuery == null ? await _playlistsRepository.GetAllFromPageAsync(pageNumber, countPerPage)
           : await _playlistsRepository.GetAllFromPageAsync(pageNumber, countPerPage,
               x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
       var playlistDtos = await Normalize(pageResult.ItemsOnPage);
       return new PageResult<PlaylistDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
           pageResult.TotalItemsCount, playlistDtos);
   }

   public async Task<IReadOnlyCollection<PlaylistDto>> GetUserPlaylists(Guid userId)
   {
       _ = await _usersRepository.GetAsync(userId)
           ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

       var playlists = await _playlistsRepository.GetAllAsync(x => x.OwnerId == userId);
       return await Normalize(playlists);
   }

   public async Task<IReadOnlyCollection<PlaylistDto>> GetAll(long count = 30, string? searchQuery = null)
    {
        if (searchQuery == null)
        {
            var playlists = await _playlistsRepository.GetAllAsync();
            return await Normalize(playlists);
        }

        var page = await _playlistsRepository.GetAllFromPageAsync(1, count,
            x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
        return await Normalize(page.ItemsOnPage);
    }

    public async Task<PlaylistDto> Get(Guid playlistId)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                    ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        return await Normalize(playlist);
    }

    public async Task<PlaylistDto> Create(PlaylistCreateRequest request, Guid userId, HttpContext context)
    {
        _ = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var playlist = new Playlist()
        {
            Title = request.Title,
            OwnerId = userId,
            IsPublic = request.IsPublic
        };

        await _playlistsRepository.CreateAsync(playlist);

        await _publishEndpoint.PublishPlaylistCreated(playlist);

        return await Normalize(playlist);
    }

    public async Task ChangePlaylistTitle(Guid playlistId, string title, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId) 
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        
        if (!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");

        playlist.Title = title;
        
        await _playlistsRepository.UpdateAsync(playlist);
        await _publishEndpoint.PublishPlaylistTitleChanged(playlist);
    }
    
    public async Task ChangePlaylistPrivacy(Guid playlistId, bool isPublic, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId) 
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        
        if (!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");

        playlist.IsPublic = isPublic;
        
        await _playlistsRepository.UpdateAsync(playlist);
        await _publishEndpoint.PublishPlaylistPrivacyChanged(playlist);
    }

    public async Task Delete(Guid playlistId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId);
        if(playlist == null) return;
        
        if (!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");
        
        await _playlistsRepository.RemoveAsync(playlistId);

        await _publishEndpoint.PublishPlaylistDeleted(playlist);
    }
    
    private async Task<PlaylistDto> Normalize(IPlaylistMongo model)
    {
        var allModels = await _playlistNormalizeHelper.GetModels(model);
        return model.Normalize(allModels, _serviceSettings);
    }
    
    private async Task<IReadOnlyCollection<PlaylistDto>> Normalize(IReadOnlyCollection<IPlaylistMongo> models)
    {
        var allModels = await _playlistNormalizeHelper.GetModels(models); 
        return models.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
    }

    public async Task AddTrackToPlaylist(Guid playlistId, Guid trackId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
            ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        
        _ = await _tracksRepository.GetAsync(trackId)
            ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        
        if(!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");
        
        if(playlist.Tracks.Contains(trackId)) return;

        playlist.Tracks.Add(trackId);

        await _playlistsRepository.UpdateAsync(playlist);

        await _publishEndpoint.PublishPlaylistTrackAdded(playlist, trackId);
    }
    
    public async Task RemoveTrackFromPlaylist(Guid playlistId, Guid trackId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        
        _ = await _tracksRepository.GetAsync(trackId)
            ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);
        
        if(!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");
        
        if(!playlist.Tracks.Contains(trackId)) return;

        playlist.Tracks.Remove(trackId);

        await _playlistsRepository.UpdateAsync(playlist);

        await _publishEndpoint.PublishPlaylistTrackRemoved(playlist, trackId);
    }

    public async Task AddTracksFromArtist(Guid playlistId, Guid artistToAddId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        var artist = await _artistsRepository.GetAsync(artistToAddId)
                     ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        if(!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");

        var trackIds = (await _tracksRepository.GetAllAsync(x => x.OwnerId == artist.Id || x.Artists.Contains(artist.Id)))
            .Select(x => x.Id).Where(x => !playlist.Tracks.Contains(x)).ToArray();
        
        foreach (var track in trackIds)
        {
            playlist.Tracks.Add(track);
        }

        await _playlistsRepository.UpdateAsync(playlist);
        foreach (var trackId in trackIds)
        {
            await _publishEndpoint.PublishPlaylistTrackAdded(playlist, trackId);
        }
    }

    public async Task AddTracksFromAlbum(Guid playlistId, Guid albumToAddId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        var albumToAdd = await _albumsRepository.GetAsync(albumToAddId)
                     ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        if(!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");
        
        var trackIds = (await _tracksRepository.GetAllAsync(albumToAdd.Tracks))
            .Select(x => x.Id).Where(x => !playlist.Tracks.Contains(x)).ToArray();
        
        foreach (var track in trackIds)
        {
            playlist.Tracks.Add(track);
        }
        
        await _playlistsRepository.UpdateAsync(playlist);
        foreach (var trackId in trackIds)
        {
            await _publishEndpoint.PublishPlaylistTrackAdded(playlist, trackId);
        }
    }

    public async Task AddTracksFromPlaylist(Guid playlistId, Guid playlistToAddId, Guid userId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        var playlistToAdd = await _playlistsRepository.GetAsync(playlistToAddId)
                       ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);
        if(!context.IsAdmin() && playlist.OwnerId != userId)
            throw new ForbiddenException("User have no rights to this playlist");
        
        var trackIds = (await _tracksRepository.GetAllAsync(playlistToAdd.Tracks))
            .Select(x => x.Id).Where(x => !playlist.Tracks.Contains(x)).ToArray();
        
        foreach (var track in trackIds)
        {
            playlist.Tracks.Add(track);
        }
        
        await _playlistsRepository.UpdateAsync(playlist);
        foreach (var trackId in trackIds)
        {
            await _publishEndpoint.PublishPlaylistTrackAdded(playlist, trackId);
        }
    }
}