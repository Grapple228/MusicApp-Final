using MassTransit;
using Music.Identity.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Identity.Exceptions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.Identity.Common;
using static System.Enum;

namespace Music.Identity.Service.Services;

public interface IRolesService
{
    Task AddRole(Guid userId, Roles role);
    Task RemoveRole(Guid userId, Roles role);
    Task<Role> GetRole(Roles userRole);
    Task<Role> GetRole(string roleValue);
    Task<IReadOnlyCollection<Role>> GetRoles(IEnumerable<Roles> roles);
    Task<IReadOnlyCollection<Role>> GetRoles(IEnumerable<Guid> roleIds);
    Task<IReadOnlyCollection<Roles>> GetUserRoles(User user);
    Task<IReadOnlyCollection<Roles>> GetUserRoles(Guid userId);
}

public class RolesService : IRolesService
{
    private readonly IRepository<Role> _rolesRepository;
    private readonly IRepository<User> _usersRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RolesService(IRepository<Role> rolesRepository, IRepository<User> usersRepository,
        IPublishEndpoint publishEndpoint)
    {
        _rolesRepository = rolesRepository;
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task AddRole(Guid userId, Roles role)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var newRole = await GetRole(role);

        if (user.Roles.Contains(newRole.Id))
            return;

        user.Roles.Add(newRole.Id);
        await _usersRepository.UpdateAsync(user);
        
        if (TryParse<Roles>(newRole.Value, out var r) && r == Roles.Artist)
        {
            await _publishEndpoint.PublishArtistCreated(new ArtistMongoBase(){ Id = user.Id, Name = user.Username });
        }
    }

    public async Task RemoveRole(Guid userId, Roles role)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var roleToRemove = await GetRole(role);

        if (!user.Roles.Contains(roleToRemove.Id)) return;
        
        user.Roles.Remove(roleToRemove.Id);
        await _usersRepository.UpdateAsync(user);

        if (TryParse<Roles>(roleToRemove.Value, out var r) && r == Roles.Artist)
        {
            await _publishEndpoint.PublishArtistDeleted(new ArtistMongoBase(){ Id = user.Id });
        }
    }
    
    public async Task<Role> GetRole(string roleValue)
    {
        var role = await _rolesRepository.GetAsync(x => x.Value == roleValue)
            ?? throw new NotFoundException(IdentityExceptionMessages.RoleNotFound);
        return role;
    }
    
    public async Task<Role> GetRole(Roles userRole)
    {
        var role = await _rolesRepository.GetAsync(x => x.Value == userRole.ToString());
        if (role != null) return role;
        
        role = new Role()
        {
            Value = userRole.ToString(),
        };
        await _rolesRepository.CreateAsync(role);

        return role;
    }

    public async Task<IReadOnlyCollection<Role>> GetRoles(IEnumerable<Roles> roles)
    {
        var roleStrings = roles.Select(x => x.ToString());
        var resultRoles = (await _rolesRepository.GetAllAsync(x => roleStrings.Contains(x.Value))).ToList();

        var notExistRoles = roleStrings.Where(x => resultRoles.All(r => r.Value != x));
        foreach (var notExistRole in notExistRoles)
        {
            var role = new Role
            {
              Value  = notExistRole,
            };
            await _rolesRepository.CreateAsync(role);
            resultRoles.Add(role);
        }

        return resultRoles;
    }

    public async Task<IReadOnlyCollection<Role>> GetRoles(IEnumerable<Guid> roleIds)
    {
        return await _rolesRepository.GetAllAsync(x => roleIds.Contains(x.Id));
    }

    public async Task<IReadOnlyCollection<Roles>> GetUserRoles(Guid userId)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);
        return await GetUserRoles(user);
    }
    
    public async Task<IReadOnlyCollection<Roles>> GetUserRoles(User user)
    {
        var userRoles = (await _rolesRepository.GetAllAsync(x => user.Roles.Contains(x.Id))).Select(x =>
            {
                _ = TryParse<Roles>(x.Value, out var role);
                return role;
            })
            .ToArray();
        if (userRoles.Length != 0) return userRoles;
        
        var role = await GetRole(Roles.User);
        user.Roles = new[] { role.Id };
        await _usersRepository.UpdateAsync(user);
        _ = TryParse<Roles>(role.Value, out var roleEnum);
        return new[] { roleEnum };
    }
}