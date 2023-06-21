using MassTransit;
using Music.Albums.Service.Helpers;
using Music.Albums.Service.Models;
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
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Requests.Albums;

namespace Music.Albums.Service.Services;

public interface IAlbumsService
{
    Task<PageResult<AlbumDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null);
    Task<IReadOnlyCollection<AlbumDto>> GetAll(long count = long.MaxValue, string? searchQuery = null);
    Task<IReadOnlyCollection<AlbumDto>> GetUserAlbums(Guid userId);
    Task<AlbumDto> Get(Guid albumId);
    Task<AlbumDto> Create(Guid creatorId, AlbumCreateRequest request, HttpContext context);
    Task ChangeAlbumPublicationDate(Guid albumId, DateOnly publicationDate, Guid userId, HttpContext context);
    Task ChangeAlbumTitle(Guid albumId, string title, Guid userId, HttpContext context);
    Task ChangeAlbumPrivacy(Guid albumId, bool isPublic, Guid userId, HttpContext context);
    Task Delete(Guid albumId, Guid userId, HttpContext context);
    Task AddTrackToAlbum(Guid albumId, Guid trackId, Guid userId, HttpContext context);
    Task RemoveTrackFromAlbum(Guid albumId, Guid trackId, Guid userId, HttpContext context);
    Task AddArtistToAlbum(Guid albumId, Guid artistId, Guid userId, HttpContext context);
    Task RemoveArtistFromAlbum(Guid albumId, Guid artistId, Guid userId, HttpContext context);
}

public class AlbumsService : IAlbumsService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ServiceSettings _serviceSettings;
    private readonly IAlbumNormalizeHelper _albumNormalizeHelper;
    private readonly IRepository<Album> _albumsRepository;
    private readonly IRepository<TrackMongoBase> _tracksRepository;
    private readonly IRepository<ArtistMongoBase> _artistsRepository;

    public AlbumsService(
        IPublishEndpoint publishEndpoint, 
        ServiceSettings serviceSettings,
        IAlbumNormalizeHelper albumNormalizeHelper, 
        IRepository<Album> albumsRepository, 
        IRepository<TrackMongoBase> tracksRepository,
        IRepository<ArtistMongoBase> artistsRepository)
    {
        _publishEndpoint = publishEndpoint;
        _serviceSettings = serviceSettings;
        _albumNormalizeHelper = albumNormalizeHelper;
        _albumsRepository = albumsRepository;
        _tracksRepository = tracksRepository;
        _artistsRepository = artistsRepository;
    }

    public async Task<PageResult<AlbumDto>> GetPage(long pageNumber, long countPerPage, string? searchQuery = null)
    {
        var pageResult = searchQuery == null ? await _albumsRepository.GetAllFromPageAsync(pageNumber, countPerPage)
            : await _albumsRepository.GetAllFromPageAsync(pageNumber, countPerPage,
                x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
        var albumDtos = await Normalize(pageResult.ItemsOnPage);
        return new PageResult<AlbumDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
            pageResult.TotalItemsCount, albumDtos);
    }

    public async Task<IReadOnlyCollection<AlbumDto>> GetAll(long count = 9223372036854775807, string? searchQuery = null)
    {
        if (searchQuery == null)
        {
            var albums = await _albumsRepository.GetAllAsync();
            return await Normalize(albums);
        }

        var page = await _albumsRepository.GetAllFromPageAsync(1, count,
            x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower()));
        return await Normalize(page.ItemsOnPage);
    }

    public async Task<IReadOnlyCollection<AlbumDto>> GetUserAlbums(Guid userId)
    {
        var albums = await _albumsRepository.GetAllAsync(x => x.OwnerId == userId || x.Artists.Contains(userId));
        return (await Normalize(albums));
    }

    public async Task<AlbumDto> Get(Guid albumId)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        return await Normalize(album);
    }

    public async Task<AlbumDto> Create(Guid creatorId, AlbumCreateRequest request, HttpContext context)
    {
        _ = await _artistsRepository.GetAsync(creatorId)
            ?? throw new ForbiddenException("Artist not found!");

        var album = new Album
        {
            Title = request.Title,
            PublicationDate = request.PublicationDate,
            OwnerId = creatorId,
        };

        await _albumsRepository.CreateAsync(album);

        await _publishEndpoint.PublishAlbumCreated(album);

        return await Normalize(album);
    }

    public async Task ChangeAlbumPublicationDate(Guid albumId, DateOnly publicationDate, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId) 
                            ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        
        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this album");

        album.PublicationDate = publicationDate;
        await _albumsRepository.UpdateAsync(album);
        await _publishEndpoint.PublishAlbumPublicationDateChanged(album);
    }

    public async Task ChangeAlbumTitle(Guid albumId, string title, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId) 
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        
        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this album");

        album.Title = title;
        await _albumsRepository.UpdateAsync(album);
        await _publishEndpoint.PublishAlbumTitleChanged(album);
    }

    public async Task ChangeAlbumPrivacy(Guid albumId, bool isPublic, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId) 
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        
        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this album");

        album.IsPublic = isPublic;
        await _albumsRepository.UpdateAsync(album);
        await _publishEndpoint.PublishAlbumPrivacyChanged(album);
    }


    public async Task Delete(Guid albumId, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId);
        if(album == null) return;

        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have ho rights to this album");
        
        await _albumsRepository.RemoveAsync(albumId);
        await _publishEndpoint.PublishAlbumDeleted(album);
    }
    
    private async Task<AlbumDto> Normalize(IAlbumMongo model)
    {
        var allModels = await _albumNormalizeHelper.GetModels(model);
        return model.Normalize(allModels, _serviceSettings);
    }
    
    private async Task<IReadOnlyCollection<AlbumDto>> Normalize(IReadOnlyCollection<IAlbumMongo> models)
    {
        var allModels = await _albumNormalizeHelper.GetModels(models); 
        return models.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
    }

    public async Task AddTrackToAlbum(Guid albumId, Guid trackId, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin())
        {
            if (!album.Artists.Contains(userId) && album.OwnerId != userId)
                throw new ForbiddenException("Artist have no access to this album");

            if(track.OwnerId != userId)
                throw new ForbiddenException("Artist have no access to this track");
        }

        if(album.Tracks.Contains(trackId)) return;
        album.Tracks.Add(trackId);

        await _albumsRepository.UpdateAsync(album);

        await _publishEndpoint.PublishAlbumTrackAdded(album, trackId);
    }
    
    public async Task RemoveTrackFromAlbum(Guid albumId, Guid trackId, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin())
        {
            var isOwner = album.OwnerId == userId;
            if (!album.Artists.Contains(userId) && !isOwner)
                throw new ForbiddenException("Artist have no access to this album");
        
            if(track.OwnerId != userId && !isOwner)
                throw new ForbiddenException("Artist have no access to this track");
        }

        if(!album.Tracks.Contains(trackId)) return;
        
        album.Tracks.Remove(trackId);
        
        await _albumsRepository.UpdateAsync(album);

        await _publishEndpoint.PublishAlbumTrackRemoved(album, trackId);
    }

    public async Task AddArtistToAlbum(Guid albumId, Guid artistId, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        
        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have no access to this album");

        if (album.OwnerId == artistId) return;
        
        if(album.Artists.Contains(artistId)) return;

        album.Artists.Add(artistId);

        await _albumsRepository.UpdateAsync(album);
        
        await _publishEndpoint.PublishAlbumArtistAdded(album, artistId);
    }
    
    public async Task RemoveArtistFromAlbum(Guid albumId, Guid artistId, Guid userId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);
        
        if(!context.IsAdmin() && album.OwnerId != userId)
            throw new ForbiddenException("Artist have no access to this album");

        if (album.OwnerId == artistId)
            throw new ForbiddenException("Can't remove album owner");
        
        if(!album.Artists.Contains(artistId)) return;

        album.Artists.Remove(artistId);

        await _albumsRepository.UpdateAsync(album);
        
        await _publishEndpoint.PublishAlbumArtistRemoved(album, artistId);
    }
}