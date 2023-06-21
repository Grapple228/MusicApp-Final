using Music.Shared.Common;

namespace Music.Tracks.Service.Models;

public class RoomUser : IModel
{
    public Guid Id { get; init; }
    public List<string> ConnectionIds { get; set; } = new();
}