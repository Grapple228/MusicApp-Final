using System.Collections.ObjectModel;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Models.Media.Users;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;

namespace Music.Applications.Windows.Models;

public class Room : IModel
{
    public Room(RoomDto roomDto)
    {
        Id = roomDto.Id;
        RoomCode = roomDto.RoomCode;
        CreationDate = roomDto.CreationDate;
        OwnerId =roomDto.OwnerId;
        CurrentTrackId =roomDto.CurrentTrackId;
        IsPlaying =roomDto.IsPlaying;
        Position =roomDto.Position;
        PositionUpdateTime = roomDto.PositionUpdateTime;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        
        foreach (var userShortDto in roomDto.Users)
        {
            Users.Add(new User(userShortDto));
        }

        foreach (var track in roomDto.TracksQuery)
        {
            Tracks.Add(new Track(track));
        }
    }
    
    public Guid Id { get; init; }
    public string RoomCode { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public Guid OwnerId { get; set; }
    public ObservableCollection<User> Users { get; } = new();
    public Guid CurrentTrackId { get; set; }
    public bool IsPlaying { get; set; }
    public long Position { get; set; }
    public DateTimeOffset PositionUpdateTime { get; set; }
    public bool IsUserOwner { get; set; }
    public List<Track> Tracks { get; } = new();
}