using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Requests.Genres;

namespace MusicClient.Api.Genres;

public interface IGenresApi
{
    Task<GenreDto> Get(Guid genreId);
    Task<PageResult<GenreDto>> GetAllPaged(long pageNumber, long countPerPage);
    Task<IReadOnlyCollection<GenreDto>> GetAll();
    Task<GenreDto> Create(GenreCreateRequest createRequest);
    Task<GenreDto> Update(Guid genreId, GenreUpdateRequest updateRequest);
    Task Delete(Guid genreId);
}