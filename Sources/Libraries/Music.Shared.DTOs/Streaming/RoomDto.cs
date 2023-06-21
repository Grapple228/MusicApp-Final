using Music.Shared.Common;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;

namespace Music.Shared.DTOs.Streaming;

public record RoomDto(
    Guid Id, 
    string RoomCode, 
    DateTimeOffset CreationDate, 
    Guid OwnerId, 
    IReadOnlyCollection<UserShortDto> Users,
    Guid CurrentTrackId, 
    bool IsPlaying, 
    long Position, 
    DateTimeOffset PositionUpdateTime,
    IReadOnlyCollection<TrackDto> TracksQuery) : IModel;