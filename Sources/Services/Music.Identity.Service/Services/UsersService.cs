using MassTransit;
using MongoDB.Driver.Linq;
using Music.Identity.Service.Models;
using Music.Identity.Service.Tools;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Services.Models;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Services;

namespace Music.Identity.Service.Services;

public interface IUsersService
{
    Task DeleteUser(Guid userId);
    Task ChangeUsername(string username, Guid userId);
    Task CreateDefaultUsers();
    Task<IdentityUserDto> GetUser(Guid userId);
    Task<IEnumerable<IdentityUserDto>> GetUsers();
}

public class UsersService : IUsersService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRepository<User> _usersRepository;
    private readonly IRepository<Password> _passwordsRepository;
    private readonly IRepository<Token> _tokensRepository;
    private readonly IRepository<Email> _emailsRepository;
    private readonly IRolesService _rolesService;
    private readonly ServiceSettings _serviceSettings;

    public UsersService(IPublishEndpoint publishEndpoint, IRepository<User> usersRepository, IRepository<Password> passwordsRepository,
        IRepository<Token> tokensRepository, IRepository<Email> emailsRepository, IRolesService rolesService, ServiceSettings serviceSettings)
    {
        _publishEndpoint = publishEndpoint;
        _usersRepository = usersRepository;
        _passwordsRepository = passwordsRepository;
        _tokensRepository = tokensRepository;
        _emailsRepository = emailsRepository;
        _rolesService = rolesService;
        _serviceSettings = serviceSettings;
    }

    public async Task<IEnumerable<IdentityUserDto>> GetUsers()
    {
        var users = await _usersRepository.GetAllAsync();
        var emailIds = users.Select(x => x.EmailId);
        var emails = await _emailsRepository.GetAllAsync(x => emailIds.Contains(x.Id));
        var roleIds = users.SelectMany(x => x.Roles).Distinct();
        var roles = await _rolesService.GetRoles(roleIds);
        var shortRoles = roles.Select(x => new ShortRole(x.Id, Enum.Parse<Roles>(x.Value)));
        return users.Select(user => GetIdentityUser(user, emails, shortRoles));
    }

    private IdentityUserDto GetIdentityUser(User user, IEnumerable<Email> emails, IEnumerable<ShortRole> roles)
    {
        var email = emails.First(x => x.Id == user.EmailId);
        return new(user.Id, user.Username, email?.Value, email?.IsConfirmed ?? false, roles.Where(x => user.Roles.Contains(x.Id)).Select(x => x.Role),
            user.RegistrationDate, new ImagePath(ServiceNames.Users, _serviceSettings.GatewayPath).GetPath(user.Id));
    }
    private IdentityUserDto GetIdentityUser(User user, Email? email, IEnumerable<Roles> roles) =>
        new(user.Id, user.Username, email?.Value, email?.IsConfirmed ?? false, roles,
            user.RegistrationDate, new ImagePath(ServiceNames.Users, _serviceSettings.GatewayPath).GetPath(user.Id));
    
    public async Task<IdentityUserDto> GetUser(Guid userId)
    {
        var user = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
        var email = await _emailsRepository.GetAsync(user.EmailId);
        var roles = (await _rolesService.GetUserRoles(user));
        return GetIdentityUser(user, email, roles);
    }

    public async Task CreateDefaultUsers()
    {
        const string username = "Admin";
        var id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var user = await _usersRepository.GetAsync(id);

        var roles = await _rolesService.GetRoles(new[] { Roles.Admin, Roles.User });

        if (user != null)
        {
            var isChanged = false;
            
            foreach (var role in roles)
            {
                if (user.Roles.Contains(role.Id)) continue;
                user.Roles.Add(role.Id);
                isChanged = true;
            }
            
            if(isChanged) await _usersRepository.UpdateAsync(user);
            await _publishEndpoint.PublishUserCreated(new UserMongoBase{ Id = user.Id, Username = user.Username});
            return;
        }

        const string emailString = "admin@yandex.ru";
        var email = await _emailsRepository.GetAsync(x => x.Value == emailString);
        if (email == null)
        {
            email = new Email
            {
                Value = emailString,
                IsConfirmed = true
            };
            await _emailsRepository.CreateAsync(email);
        }
        
        user = new User()
        {
            Id = id,
            Roles = roles.Select(x => x.Id).ToList(),
            Username = username,
            EmailId = email.Id
        };

        var password = await _passwordsRepository.GetAsync(x => x.UserId == id);
        if (password != null)
            await _passwordsRepository.RemoveAsync(password.Id);
        password = PasswordProcessor.GenHashedPassword(user.Id, "password");
        await _passwordsRepository.CreateAsync(password);
        
        await _usersRepository.CreateAsync(user);
        await _publishEndpoint.PublishUserCreated(new UserMongoBase
        {
            Id = user.Id,
            Username = user.Username
        });
    }
    
    public async Task DeleteUser(Guid userId)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        if ((await _rolesService.GetUserRoles(userId)).Contains(Roles.Artist))
        {
            await _publishEndpoint.PublishArtistDeleted(new ArtistMongoBase() { Id = user.Id });
        }
        
        await _usersRepository.RemoveAsync(userId);
        await _publishEndpoint.PublishUserDeleted( new UserMongoBase(){ Id = user.Id} );

        await _emailsRepository.RemoveAsync(user.EmailId);

        var tokens = await _tokensRepository.GetAllAsync(x => x.UserId == userId);
        foreach (var token in tokens)
        {
            await _tokensRepository.RemoveAsync(token.Id);
        }
        var password = await _passwordsRepository.GetAsync(x => x.UserId == userId);
        if (password != null)
        {
            await _passwordsRepository.RemoveAsync(password.Id);
        }
    }

    public async Task ChangeUsername(string username, Guid userId)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
        
        var existing = await _usersRepository.GetAsync(x => x.Username.Trim().ToLower() == username.Trim().ToLower() && x.Id != user.Id);
        if (existing != null) throw new ForbiddenException("Username already used");
        
        user.Username = username.Trim();
        
        await _usersRepository.UpdateAsync(user);
        await _publishEndpoint.PublishUserUsernameChanged(new UserMongoBase()
        { Id = user.Id, Username = user.Username });
    }
}