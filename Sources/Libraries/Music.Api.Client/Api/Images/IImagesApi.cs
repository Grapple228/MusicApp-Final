using Music.Shared.Files.Common;

namespace MusicClient.Api.Images;

public interface IImagesApi
{
    Task ChangeTrack(Guid trackId, string filename);
    Task ChangeAlbum(Guid albumId, string filename);
    Task ChangeArtist(Guid artistId, string filename);
    Task ChangePlaylist(Guid playlistId, string filename);
    Task ChangeUser(Guid userId, string filename);

    Task<byte[]> GetImage(string serviceName, Guid modelId, ImageSizeEnum size);
}