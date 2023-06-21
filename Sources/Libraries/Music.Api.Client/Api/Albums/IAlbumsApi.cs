using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Albums;

namespace MusicClient.Api.Albums;

public interface IAlbumsApi : IMediaApi<MediaAlbumDto>
{
    Task<AlbumDto> Get(Guid albumId);
    Task<PageResult<AlbumDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<AlbumDto>> GetAll(long count = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<AlbumDto>> GetAllUser();
    Task<AlbumDto> Create(AlbumCreateRequest albumCreateRequest, Guid? artistId = null);
    Task Delete(Guid albumId);
    Task ChangeTitle(Guid albumId, string title);
    Task ChangePublicity(Guid albumId, bool isPublic);
    Task ChangePublicationDate(Guid albumId, DateOnly publicationDate);
    Task AddTrack(Guid albumId, Guid trackId);
    Task RemoveTrack(Guid albumId, Guid trackId);
    Task AddArtist(Guid albumId, Guid artistId);
    Task RemoveArtist(Guid albumId, Guid artistId);
}