using Music.Services.DTOs.Extensions.Converters;

namespace Music.Tracks.Service.Models;

public class RoomBase : Room
{
    public List<Guid> RoomUserIds { get; set; } = new();
    public List<Guid> TracksQuery { get; set; } = new();
}