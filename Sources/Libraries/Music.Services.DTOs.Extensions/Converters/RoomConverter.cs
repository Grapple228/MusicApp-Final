using Music.Shared.Common;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Services.DTOs.Extensions.Converters;

public class Room : IModel
{
    public Guid Id { get; init; }
    public string RoomCode { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public Guid OwnerId { get; set; }
    public List<Guid> Users { get; set; } = new();
    public Guid CurrentTrackId { get; set; }
    public Guid CurrentContainerId { get; set; }
    public ContainerType ContainerType { get; set; }
    public bool IsPlaying { get; set; }
    public long Position { get; set; }
    public DateTimeOffset PositionUpdateTime { get; set; }
}

public static class RoomConverter
{
    public static RoomDto AsDto(this Room room, IReadOnlyCollection<UserShortDto> users, IReadOnlyCollection<TrackDto> trackDtos) =>
        new(room.Id, room.RoomCode, room.CreationDate, room.OwnerId, users, room.CurrentTrackId,
            room.IsPlaying, room.Position, room.PositionUpdateTime, trackDtos);
}