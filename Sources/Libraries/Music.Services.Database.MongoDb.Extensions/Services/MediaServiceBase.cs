using System.Linq.Expressions;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Normalizers.Media;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Models.Media;
using Music.Shared.Common;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Services.Database.MongoDb.Extensions.Services;

public class MediaServiceBase<TMediaDto, TMediaInfo, TModel, TUser> : IMediaServiceBase<TMediaDto> 
    where TMediaInfo : IMediaModelBase, new()
    where TMediaDto : IModel
    where TModel : IModel
    where TUser : UserMongoBase
{
    private readonly IRepository<TUser> _usersRepository;
    private readonly IMediaNormalizer<TMediaDto, TMediaInfo, TModel> _mediaNormalizer;
    private readonly IRepository<TModel> _modelsRepository;
    private readonly IRepository<TMediaInfo> _infosRepository;

    public MediaServiceBase(IRepository<TUser> usersRepository, IMediaNormalizer<TMediaDto, TMediaInfo, TModel> mediaNormalizer,
        IRepository<TModel> modelsRepository, IRepository<TMediaInfo> infosRepository)
    {
        _usersRepository = usersRepository;
        _mediaNormalizer = mediaNormalizer;
        _modelsRepository = modelsRepository;
        _infosRepository = infosRepository;
    }
    
    protected async Task CheckUser(Guid userId)
    {
        _ = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
    }
    
    public virtual async Task<TMediaDto> Get(Guid mediaId, Guid userId)
    {
        await CheckUser(userId);
        var album = await _modelsRepository.GetAsync(mediaId)
                    ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        return await _mediaNormalizer.Normalize(album, userId);
    }

    public virtual async Task<IEnumerable<TMediaDto>> GetAll(IEnumerable<Guid> ids, Guid userId, bool isUser = false)
    {
        await CheckUser(userId);
        
        if (isUser)
        {
            var infos = await _infosRepository.GetAllAsync(x => x.UserId == userId && ids.Contains(x.MediaId));
            return await _mediaNormalizer.Normalize(infos, userId);
        }
        
        var models = await _modelsRepository.GetAllAsync(x => ids.Contains(x.Id));
        return await _mediaNormalizer.Normalize(models, userId);
    }
    
    public virtual async Task<IEnumerable<TMediaDto>> GetAll(Guid userId, bool isUser = false)
    {
        await CheckUser(userId);

        if (isUser)
        {
            var infos = await _infosRepository.GetAllAsync(x => x.UserId == userId);
            return await _mediaNormalizer.Normalize(infos, userId);
        }

        var models = await _modelsRepository.GetAllAsync();
        return await _mediaNormalizer.Normalize(models, userId);
    }

    public virtual async Task<IEnumerable<TMediaDto>> GetAllLiked(Guid userId)
    {
        await CheckUser(userId);
        var infos = await _infosRepository.GetAllAsync(x => x.UserId == userId && x.IsLiked);
        return await _mediaNormalizer.Normalize(infos, userId);
    }

    public virtual async Task<IEnumerable<TMediaDto>> GetAllBlocked(Guid userId)
    {
        await CheckUser(userId);
        var infos = await _infosRepository.GetAllAsync(x => x.UserId == userId && x.IsBlocked);
        return await _mediaNormalizer.Normalize(infos, userId);
    }

    public virtual async Task<PageResult<TMediaDto>> GetPage(Guid userId, long pageNumber, long countPerPage, bool isUser = false)
    {
        await CheckUser(userId);
        if (isUser)
        {
            var infos = await _infosRepository.GetAllFromPageAsync(pageNumber, countPerPage, x => x.UserId == userId);
            var albums = await _mediaNormalizer.Normalize(infos.ItemsOnPage, userId);
            return new PageResult<TMediaDto>(infos.TotalPagesCount, infos.CurrentPageNumber, infos.TotalItemsCount, albums);
        }
        else
        {
            var models = await _modelsRepository.GetAllFromPageAsync(pageNumber, countPerPage);
            var albums = await _mediaNormalizer.Normalize(models.ItemsOnPage, userId);
            return new PageResult<TMediaDto>(models.TotalPagesCount, models.CurrentPageNumber, models.TotalItemsCount, albums);
        }
    }

    public virtual async Task<PageResult<TMediaDto>> GetPageLiked(Guid userId, long pageNumber, long countPerPage)
    {
        await CheckUser(userId);
        var page = await _infosRepository.GetAllFromPageAsync(pageNumber, countPerPage,
            x => x.UserId == userId && x.IsLiked);
        var albums = await _mediaNormalizer.Normalize(page.ItemsOnPage, userId);
        return new PageResult<TMediaDto>(page.TotalPagesCount, page.CurrentPageNumber, page.TotalItemsCount, albums);
    }

    public virtual async Task<PageResult<TMediaDto>> GetPageBlocked(Guid userId, long pageNumber, long countPerPage)
    {
        await CheckUser(userId);
        var page = await _infosRepository.GetAllFromPageAsync(pageNumber, countPerPage,
            x => x.UserId == userId && x.IsBlocked);
        var albums = await _mediaNormalizer.Normalize(page.ItemsOnPage, userId);
        return new PageResult<TMediaDto>(page.TotalPagesCount, page.CurrentPageNumber, page.TotalItemsCount, albums);
    }

    public virtual async Task ChangeMedia(Guid mediaId, MediaCreateRequest request, Guid userId)
    {
        _ = await _modelsRepository.GetAsync(mediaId)
            ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);
        _ = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
        
        var info = await _infosRepository.GetAsync(x => x.MediaId == mediaId && x.UserId == userId);

        if (request is { IsBlocked: false, IsLiked: false })
        {
            if(info == null) return;
            await _infosRepository.RemoveAsync(info.Id);
        }
        else
        {
            if (info == null)
            {
                info = MediaCreator<TMediaInfo>.Create(mediaId, userId, request.IsLiked, request.IsBlocked);
                await _infosRepository.CreateAsync(info);
            }
            else
            {
                info.IsBlocked = request.IsBlocked;
                info.IsLiked = request.IsLiked;
                await _infosRepository.UpdateAsync(info);
            }
        }
    }
}
