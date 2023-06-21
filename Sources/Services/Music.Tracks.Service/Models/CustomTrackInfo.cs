using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;

namespace Music.Tracks.Service.Models;

public class CustomTrackInfo : TrackInfo
{
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}