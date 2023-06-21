using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;

namespace Music.Playlists.Service.Models;

public class CustomPlaylistInfo : PlaylistInfo
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}