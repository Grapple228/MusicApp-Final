using Music.Shared.Common;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Media.Models;

namespace MusicClient.Api.Artists;

public interface IArtistsApi : IMediaApi<MediaArtistDto>
{
    Task<ArtistDto> Get(Guid artistId);
    Task<PageResult<ArtistDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<ArtistDto>> GetAll(long count = 30, string? searchQuery = null);
}