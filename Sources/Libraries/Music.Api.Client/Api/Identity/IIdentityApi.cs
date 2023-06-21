using Music.Shared.Identity.Common;
using Music.Shared.Identity.Common.DTOs;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Identity.Common.Requests;
using Music.Shared.Identity.Jwt;
using MusicClient.Models.Tokens;

namespace MusicClient.Api.Identity;

public interface IIdentityApi
{
    Task<LoginDto> RefreshToken(string refreshToken);
    Task<LoginDto> Authenticate(LoginRequest loginRequest);
    Task<RegisterDto> Register(RegisterRequest registerRequest);
    Task DeleteUser(Guid userId);
    Task DeleteUser();
    Task<IdentityUserDto> GetProfile();
    Task<IdentityUserDto> GetProfile(Guid userId);
    Task<IReadOnlyCollection<IdentityUserDto>> GetProfiles();
    Task<Guid> GetId();
    Task<IReadOnlyCollection<string>> GetRoles();
    Task<IReadOnlyCollection<string>> GetRoles(Guid userId);
    Task AddRole(Guid userId, Roles role);
    Task RemoveRole(Guid userId, Roles role);
    Task ChangePassword(ChangePasswordRequest changePasswordRequest);
    Task ChangeUsername(string username);
    Task BecomeAnArtist();
}