using Music.Shared.Identity.Common;
using Music.Shared.Identity.Common.DTOs;
using Music.Shared.Identity.Common.Models;
using Music.Shared.Identity.Common.Requests;
using Music.Shared.Identity.Jwt;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Exceptions;
using MusicClient.Helpers;
using MusicClient.Models.Tokens;
using RestSharp;

namespace MusicClient.Api.Identity;

public abstract class IdentityApiBase : ApiBase, IIdentityApi
{
    protected IdentityApiBase(IApiClient client) : base(client, ServiceNames.Identity)
    {
    }

    public virtual async Task<LoginDto> RefreshToken(string refreshToken)
    {
        var request = CreateRequest(Method.Post, additional: "RefreshToken", isUseAuthorization: false);
        // todo ДОБАВИТЬ DEVICE INFO
        var updateTokenRequest = new UpdateTokenRequest(refreshToken, new DeviceInfo(){ DeviceHash = "hash1" });
        request.AddJsonBody(updateTokenRequest);
        var response = await ExecuteRequest(request, true);
        return response.Deserialize<LoginDto>() ?? throw new NullTokenException();
    }

    public virtual async Task<LoginDto> Authenticate(LoginRequest loginRequest)
    {
        var request = CreateRequest(Method.Post, additional: "authorization", isUseAuthorization: false);
        // todo ДОБАВИТЬ DEVICE INFO
        request.AddJsonBody(loginRequest);
        var response = await ExecuteRequest(request, true);
        return response.Deserialize<LoginDto>();
    }

    public virtual async Task<RegisterDto> Register(RegisterRequest registerRequest)
    {
        var request = CreateRequest(Method.Post, additional: "registration", isUseAuthorization: false);
        request.AddJsonBody(registerRequest);
        var response = await ExecuteRequest(request);
        return response.Deserialize<RegisterDto>();
    }

    public virtual async Task DeleteUser(Guid userId)
    {
        var request = CreateRequest(Method.Delete, $"{userId}");
        await ExecuteRequest(request);
    }

    public virtual async Task<IdentityUserDto> GetProfile()
    {
        var request = CreateRequest(additional:"profile");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IdentityUserDto>();
    }

    public async Task<IdentityUserDto> GetProfile(Guid userId)
    {
        var request = CreateRequest(additional: $"{userId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IdentityUserDto>();
    }

    public async Task<IReadOnlyCollection<IdentityUserDto>> GetProfiles()
    {
        var request = CreateRequest();
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<IdentityUserDto>>();
    }

    public virtual async Task DeleteUser()
    {
        var request = CreateRequest(Method.Delete, additional:"profile");
        await ExecuteRequest(request);
    }

    public virtual async Task<Guid> GetId()
    {
        var request = CreateRequest(additional:"profile/id");
        var response = await ExecuteRequest(request);
        return response.Deserialize<Guid>();
    }

    public virtual async Task<IReadOnlyCollection<string>> GetRoles()
    {
        var request = CreateRequest(additional:"profile/roles");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<string>>();
    }
    
    public virtual async Task<IReadOnlyCollection<string>> GetRoles(Guid userId)
    {
        var request = CreateRequest(additional:$"{userId}/roles");
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<string>>();
    }
    
    public virtual async Task AddRole(Guid userId, Roles role)
    {
        var request = CreateRequest(Method.Post, additional:$"{userId}/roles/{role}");
        await ExecuteRequest(request);
    }
    
    public virtual async Task RemoveRole(Guid userId, Roles role)
    {
        var request = CreateRequest(Method.Delete, additional:$"{userId}/roles/{role}");
        await ExecuteRequest(request);
    }

    public virtual async Task ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var request = CreateRequest(Method.Post, additional: "profile/changePassword");
        request.AddJsonBody(changePasswordRequest);
        await ExecuteRequest(request);
    }
    
    public virtual async Task ChangeUsername(string username)
    {
        var request = CreateRequest(Method.Post, additional: $"profile/changeUsername/{username}");
        await ExecuteRequest(request);
    }

    public async Task BecomeAnArtist()
    {
        var request = CreateRequest(Method.Post, additional: "profile/becomeArtist");
        await ExecuteRequest(request);
    }
}