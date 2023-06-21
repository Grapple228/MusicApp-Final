using Music.Shared.Common;
using Music.Shared.DTOs.Users;
using Music.Shared.Services;
using MusicClient.Client;
using MusicClient.Helpers;

namespace MusicClient.Api.Users;

public abstract class UsersApiBase : ApiBase, IUsersApi
{
    protected UsersApiBase(IApiClient client) : base(client, ServiceNames.Users)
    {
    }

    public virtual async Task<IReadOnlyCollection<UserDto>> GetAll()
    {
        var request = CreateRequest();
        var response = await ExecuteRequest(request);
        return response.Deserialize<IReadOnlyCollection<UserDto>>();
    }

    public virtual async Task<PageResult<UserDto>> GetAllPaged(long pageNumber, long countPerPage)
    {
        var request = CreateRequest(additional: $"{pageNumber}-{countPerPage}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<PageResult<UserDto>>();
    }
    
    public virtual async Task<UserDto> Get(Guid userId)
    {
        var request = CreateRequest(additional: $"{userId}");
        var response = await ExecuteRequest(request);
        return response.Deserialize<UserDto>();
    }
}