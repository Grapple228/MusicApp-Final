using MassTransit;
using Music.Identity.Service.Models;
using Music.Identity.Service.Tools;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Identity.Common;
using Music.Services.Identity.Exceptions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Services.Models;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Common.DTOs;
using Music.Shared.Identity.Common.Requests;
using Music.Shared.Identity.Jwt;
using Music.Shared.Services;

namespace Music.Identity.Service.Services;

public interface IIdentityService
{
    Task<RegisterDto> Register(RegisterRequest request);
    Task<LoginDto> Login(LoginRequest request);
    Task ChangePassword(ChangePasswordRequest request, Guid userId, HttpContext context);
}

public class IdentityService : IIdentityService
{
    private readonly IRolesService _rolesService;
    private readonly IDevicesService _devicesService;
    private readonly IRepository<User> _usersRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRepository<Password> _passwordsRepository;
    private readonly ServiceSettings _serviceSettings;
    private readonly ITokensService _tokensService;
    private readonly IRepository<Email> _emailsRepository;

    public IdentityService(IRolesService rolesService, IDevicesService devicesService,
        IRepository<User> usersRepository, IPublishEndpoint publishEndpoint, IRepository<Password> passwordsRepository, 
        ServiceSettings serviceSettings, ITokensService tokensService, IRepository<Email> emailsRepository)
    {
        _rolesService = rolesService;
        _devicesService = devicesService;
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
        _passwordsRepository = passwordsRepository;
        _serviceSettings = serviceSettings;
        _tokensService = tokensService;
        _emailsRepository = emailsRepository;
    }

    public async Task<Password> CreatePassword(Guid userId, string password)
    {
        var result = PasswordProcessor.GenHashedPassword(userId, password);
        result.ChangingDate = DateTimeOffset.UtcNow;
        await _passwordsRepository.CreateAsync(result);
        return result;
    }

    public async Task<RegisterDto> Register(RegisterRequest request)
    {
        if (request.Password.Length < 8) throw new BadRequestException("Password length less than 8");
        
        var user = await _usersRepository.GetAsync(x => x.Username.ToLower() == request.Username.ToLower());
        if (user != null) throw new ConflictException(IdentityExceptionMessages.UsernameExists);

        var normalizedEmail = request.Email.ToLower();
        var email = await _emailsRepository.GetAsync(x => x.Value.ToLower() == normalizedEmail);
        if (email != null) throw new ConflictException(IdentityExceptionMessages.EmailExists);

        email = new Email()
        {
            Value = normalizedEmail,
            IsConfirmed = false
        };

        await _emailsRepository.CreateAsync(email);
        
        var role = await _rolesService.GetRole(Roles.User);
        user = new User
        {
            Username = request.Username,
            EmailId = email.Id,
            Roles = new[] { role.Id }
        };
        await _usersRepository.CreateAsync(user);
        
        var password = PasswordProcessor.GenHashedPassword(user.Id, request.Password);
        password.ChangingDate = DateTimeOffset.UtcNow;
        await _passwordsRepository.CreateAsync(password);
        
        await _publishEndpoint.PublishUserCreated(new UserMongoBase { Id = user.Id, Username = user.Username });
        
        return new RegisterDto(user.Id, user.Username);
    }

    public async Task<LoginDto> Login(LoginRequest request)
    {
        var user = await _usersRepository.GetAsync(x => x.Username.ToLower() == request.Username.ToLower());
        if (user == null) throw new UnauthorizedException(IdentityExceptionMessages.InvalidLoginData);
        
        var userPassword = await _passwordsRepository.GetAsync(x => x.UserId == user.Id);
        if (userPassword == null) throw new UnauthorizedException(IdentityExceptionMessages.InvalidLoginData);
        
        if (!PasswordProcessor.PasswordCompare(request.Password, userPassword))
            throw new UnauthorizedException(IdentityExceptionMessages.InvalidLoginData);

        var userRoles = await _rolesService.GetUserRoles(user);

        var device = await _devicesService.GetDevice(request.DeviceInfo);

        var tokenDto = await _tokensService.GetToken(user.Id, device.Id, userRoles);
        
        var imagePath = new ImagePath(ServiceNames.Users, _serviceSettings.GatewayPath).GetPath(user.Id);
        return new LoginDto(user.Id, user.Username, imagePath, tokenDto.JwtAccessToken, tokenDto.JwtRefreshToken, userRoles);
    }

    public async Task ChangePassword(ChangePasswordRequest request, Guid userId, HttpContext context)
    {
        var user = await _usersRepository.GetAsync(userId)
                   ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var userPassword = await _passwordsRepository.GetAsync(x => x.UserId == user.Id);

        if (userPassword == null)
        {
            if (!context.IsAdmin())
                throw new ForbiddenException("Invalid password");

            await CreatePassword(userId, request.NewPassword);
            return;
        }
        
        if (!context.IsAdmin() 
            && !PasswordProcessor.PasswordCompare(request.CurrentPassword, userPassword))
            throw new ForbiddenException("Invalid password");

        var newPassword = PasswordProcessor.GenHashedPassword(user.Id, request.NewPassword);
        userPassword.Hash = newPassword.Hash;
        userPassword.Salt = newPassword.Salt;
        userPassword.ChangingDate = DateTimeOffset.UtcNow;
        await _passwordsRepository.UpdateAsync(userPassword);
        
    }
}