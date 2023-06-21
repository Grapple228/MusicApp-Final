using MassTransit;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Extensions.Normalizers;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Shared.Common;
using Music.Shared.DTOs.Users;
using Music.Users.Service.Helpers;

namespace Music.Users.Service.Services;

public interface IUsersService
{
    Task<PageResult<UserDto>> GetPage(long pageNumber, long countPerPage);
    Task<IReadOnlyCollection<UserDto>> GetAll();
    Task<UserDto> Get(Guid userId);
}

public class UsersService : IUsersService
{
    private readonly IRepository<UserMongoBase> _usersRepository;
    private readonly ServiceSettings _serviceSettings;
    private readonly IUserNormalizeHelper _userNormalizeHelper;

    public UsersService(IRepository<UserMongoBase> usersRepository, ServiceSettings serviceSettings,
        IUserNormalizeHelper userNormalizeHelper)
    {
        _usersRepository = usersRepository;
        _serviceSettings = serviceSettings;
        _userNormalizeHelper = userNormalizeHelper;
    }
    
    public async Task<PageResult<UserDto>> GetPage(long pageNumber, long countPerPage)
    {
        var pageResult = await _usersRepository.GetAllFromPageAsync(pageNumber, countPerPage);
        var playlistDtos = await Normalize(pageResult.ItemsOnPage);
        return new PageResult<UserDto>(pageResult.TotalPagesCount, pageResult.CurrentPageNumber,
            pageResult.TotalItemsCount, playlistDtos);
    }

    public async Task<IReadOnlyCollection<UserDto>> GetAll()
    {
        var users = await _usersRepository.GetAllAsync();
        return await Normalize(users);
    }

    public async Task<UserDto> Get(Guid userId)
    {
        var user = await _usersRepository.GetAsync(userId)
                       ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
        return await Normalize(user);
    }

    private async Task<UserDto> Normalize(IUserMongo model)
    {
        var allModels = await _userNormalizeHelper.GetModels(model);
        return model.Normalize(allModels, _serviceSettings);
    }
    
    private async Task<IReadOnlyCollection<UserDto>> Normalize(IReadOnlyCollection<IUserMongo> models)
    {
        var allModels = await _userNormalizeHelper.GetModels(models); 
        return models.Select(x => x.Normalize(allModels, _serviceSettings)).ToList();
    }
}