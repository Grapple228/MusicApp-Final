using System.Linq.Expressions;
using Music.Shared.Common;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Services.DTOs.Extensions;

public interface IMediaServiceBase<TDto>
    where TDto : IModel
{
    Task<TDto> Get(Guid mediaId, Guid userId);
    Task<IEnumerable<TDto>> GetAll(Guid userId, bool isUser = false);
    Task<IEnumerable<TDto>> GetAll(IEnumerable<Guid> ids, Guid userId, bool isUser = false);
    Task<IEnumerable<TDto>> GetAllLiked(Guid userId);
    Task<IEnumerable<TDto>> GetAllBlocked(Guid userId);
    Task<PageResult<TDto>> GetPage(Guid userId, long pageNumber, long countPerPage, bool isUser = false);
    Task<PageResult<TDto>> GetPageLiked(Guid userId, long pageNumber, long countPerPage);
    Task<PageResult<TDto>> GetPageBlocked(Guid userId, long pageNumber, long countPerPage);
    Task ChangeMedia(Guid mediaId, MediaCreateRequest request, Guid userId);
}