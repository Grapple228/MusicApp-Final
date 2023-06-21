using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;

namespace Music.Artists.Service.Models;

public class CustomArtistInfo : ArtistInfo
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}