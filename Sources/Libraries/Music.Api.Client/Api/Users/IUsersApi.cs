using Music.Shared.Common;
using Music.Shared.DTOs.Users;

namespace MusicClient.Api.Users;

public interface IUsersApi
{
    Task<UserDto> Get(Guid userId);
    Task<PageResult<UserDto>> GetAllPaged(long pageNumber, long countPerPage);
    Task<IReadOnlyCollection<UserDto>> GetAll();
}