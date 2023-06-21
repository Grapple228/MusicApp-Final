using Music.Shared.Common;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Tracks;

namespace MusicClient.Api.Tracks;

public interface ITracksApi : IMediaApi<MediaTrackDto>
{
    Task<TrackDto> Get(Guid trackId);
    Task<PageResult<TrackDto>> GetAllPaged(long pageNumber, long countPerPage = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<TrackDto>> GetAll(long count = 30, string? searchQuery = null);
    Task<IReadOnlyCollection<TrackDto>> GetAllUser();
    Task<TrackDto> Create(string title, IReadOnlyCollection<Guid> genres, DateOnly publicationDate, string fileName, Guid? artistId = null);
    Task Delete(Guid trackId);
    Task ChangeTitle(Guid trackId, string title);
    Task ChangePrivacy(Guid trackId, bool isPublic);
    Task ChangePublicationDate(Guid trackId, DateOnly publicationDate);
    Task<int> ChangeFile(Guid trackId, string filename);
    Task AddGenre(Guid trackId, Guid genreId);
    Task RemoveGenre(Guid trackId, Guid genreId);
    Task AddArtist(Guid trackId, Guid artistId);
    Task RemoveArtist(Guid trackId, Guid artistId);
}