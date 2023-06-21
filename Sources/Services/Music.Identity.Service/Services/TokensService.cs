using Music.Identity.Service.Models;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Identity.Exceptions;
using Music.Services.Identity.Jwt;
using Music.Services.Models;
using Music.Shared.Identity.Common;
using Music.Shared.Identity.Jwt;
using Music.Shared.Services;

namespace Music.Identity.Service.Services;

public interface ITokensService
{
    Task<TokenDto> GetToken(Guid userId, Guid deviceId, IReadOnlyCollection<Roles> userRoles);
    Task<LoginDto> RefreshToken(UpdateTokenRequest request);
}

public class TokensService : ITokensService
{
    private readonly IdentitySettings _identitySettings;
    private readonly IRepository<Token> _tokensRepository;
    private readonly IDevicesService _devicesService;
    private readonly IRolesService _rolesService;
    private readonly IRepository<User> _usersRepository;
    private readonly ServiceSettings _serviceSettings;

    public TokensService(IdentitySettings identitySettings, IRepository<Token> tokensRepository,
        IDevicesService devicesService, IRolesService rolesService, IRepository<User> usersRepository,
        ServiceSettings serviceSettings)
    {
        _identitySettings = identitySettings;
        _tokensRepository = tokensRepository;
        _devicesService = devicesService;
        _rolesService = rolesService;
        _usersRepository = usersRepository;
        _serviceSettings = serviceSettings;
    }
    
    public async Task<TokenDto> GetToken(Guid userId, Guid deviceId, IReadOnlyCollection<Roles> userRoles)
    {
        var jwtTokenHandler = new JwtTokenHandler(_identitySettings.AccessValidityMinutes, _identitySettings.RefreshValidityDays);
        var tokenDto = jwtTokenHandler.CreateJwtToken(userId, userRoles);
        var token = await _tokensRepository.GetAsync(x => x.UserId == userId && x.DeviceId == deviceId);
        var expirationDate = DateTimeOffset.Now.AddDays(jwtTokenHandler.RefreshValidityDays);
        if (token == null)
        {
            token = new Token()
            {
                DbAddingDate = DateTimeOffset.Now,
                RefreshDate = DateTimeOffset.Now,
                ExpirationDate = expirationDate,
                RefreshToken = tokenDto.JwtRefreshToken,
                UserId = userId,
                DeviceId = deviceId
            };
            await _tokensRepository.CreateAsync(token);
        }
        else
        {
            token.ExpirationDate = expirationDate;
            token.RefreshDate = DateTimeOffset.Now;
            token.RefreshToken = tokenDto.JwtRefreshToken;
            await _tokensRepository.UpdateAsync(token);
        }

        return tokenDto;
    }

    public async Task<LoginDto> RefreshToken(UpdateTokenRequest request)
    {
        var device = await _devicesService.GetDevice(request.DeviceInfo);
        var token = await _tokensRepository.GetAsync(x => x.RefreshToken == request.RefreshToken)
            ?? throw new UnauthorizedException(IdentityExceptionMessages.InvalidRefreshToken);

        if (token.ExpirationDate < DateTimeOffset.UtcNow)
        {
            await _tokensRepository.RemoveAsync(token.Id);
            throw new UnauthorizedException(IdentityExceptionMessages.InvalidRefreshToken);
        }
        
        if (token.DeviceId != device.Id)
        {
            await _tokensRepository.RemoveAsync(token.Id);
            throw new UnauthorizedException(IdentityExceptionMessages.InvalidRefreshToken);
        }

        var user = await _usersRepository.GetAsync(x => x.Id == token.UserId);
        if (user == null) throw new UnauthorizedException(IdentityExceptionMessages.InvalidRefreshToken);
        
        var userRoles = await _rolesService.GetUserRoles(user);
        
        var jwtTokenHandler = new JwtTokenHandler(_identitySettings.AccessValidityMinutes, _identitySettings.RefreshValidityDays);
        var tokenDto = jwtTokenHandler.CreateJwtToken(user.Id, userRoles);

        token.ExpirationDate = DateTimeOffset.UtcNow.AddDays(_identitySettings.RefreshValidityDays);
        token.RefreshToken = tokenDto.JwtRefreshToken;
        token.RefreshDate = DateTimeOffset.UtcNow;
        await _tokensRepository.UpdateAsync(token);

        return new LoginDto(user.Id, user.Username, new ImagePath(ServiceNames.Users, _serviceSettings.GatewayPath).GetPath(user.Id), 
            tokenDto.JwtAccessToken, tokenDto.JwtRefreshToken, tokenDto.Roles);
    }
}