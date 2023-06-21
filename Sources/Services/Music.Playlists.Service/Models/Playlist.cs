
using Music.Services.Database.MongoDb.Models;

namespace Music.Playlists.Service.Models;

public class Playlist : PlaylistMongoBase
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}