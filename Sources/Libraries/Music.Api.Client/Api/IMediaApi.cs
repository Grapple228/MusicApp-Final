using Music.Shared.Common;
using Music.Shared.DTOs.Media.Enums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;

namespace MusicClient.Api;

public interface IMediaApi<TMediaDto> where TMediaDto : IModel
{
    Task<TMediaDto> GetMedia(Guid mediaId);
    Task<IReadOnlyCollection<TMediaDto>> GetAllMedia(MediaFilterEnum filter = MediaFilterEnum.All);
    Task<IReadOnlyCollection<TMediaDto>> GetAllUserMedia();
    Task ChangeMedia(Guid mediaId, MediaCreateRequest createRequest);
    Task<PageResult<TMediaDto>> GetAllMediaPaged(long pageNumber, long countPerPage, MediaFilterEnum filter = MediaFilterEnum.All);
    Task<PageResult<TMediaDto>> GetAllUserMediaPaged(long pageNumber, long countPerPage);
    Task<IReadOnlyCollection<MediaTrackDto>> GetTracks(Guid mediaId);
}