using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Music.Services.Common;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Models;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;
using Music.Tracks.Service.Helpers;
using Music.Tracks.Service.Models;
using Music.Tracks.Service.Services;

namespace Music.Tracks.Service.Hubs;

[Authorize]
public class StreamingHub : Hub
{
    public async Task NextTrack()
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        var index = room.TracksQuery.IndexOf(room.CurrentTrackId);
        if (index >= room.TracksQuery.Count - 1)
        {
            room.Position = 0;
            room.PositionUpdateTime = DateTimeOffset.UtcNow;
            room.IsPlaying = false;
            await _roomsRepository.UpdateAsync(room);
            await Clients.Group(room.RoomCode).SendAsync("TrackPlayRequested", room.CurrentTrackId, room.Position, room.IsPlaying);
            return;
        }
        var newTrack = room.TracksQuery[index+1];
        room.CurrentTrackId = newTrack;
        room.Position = 0;
        room.PositionUpdateTime = DateTimeOffset.UtcNow;
        room.IsPlaying = true;
        
        await _roomsRepository.UpdateAsync(room);

        await Clients.Group(room.RoomCode).SendAsync("TrackPlayRequested", newTrack, 0, true);
    }

    public async Task PrevTrack()
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        var index = room.TracksQuery.IndexOf(room.CurrentTrackId);
        if(index <= 0) return;
        var newTrack = room.TracksQuery[index-1];
        room.CurrentTrackId = newTrack;
        room.Position = 0;
        room.PositionUpdateTime = DateTimeOffset.UtcNow;
        room.IsPlaying = true;
        
        await _roomsRepository.UpdateAsync(room);

        await Clients.Group(room.RoomCode).SendAsync("TrackPlayRequested", newTrack, 0, true);
    }
    
    public async Task RequestPositionChange(long position)
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        room.Position = position;

        await _roomsRepository.UpdateAsync(room);
        
        await Clients.Group(room.RoomCode).SendAsync("PositionChanged", room.Position);
    }
    
    public async Task RequestTrackPlay(Guid trackId, long position)
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        if (!room.TracksQuery.Contains(trackId)) throw new BadRequestException("Can't play track not from query");
        
        if (room.CurrentTrackId == trackId)
        {
            room.IsPlaying = !room.IsPlaying;
            room.Position = position;
            room.PositionUpdateTime = DateTimeOffset.UtcNow;
        }
        else
        {
            room.CurrentTrackId = trackId;
            room.IsPlaying = true;
            room.Position = 0;
            room.PositionUpdateTime = DateTimeOffset.UtcNow;
        }
        
        await _roomsRepository.UpdateAsync(room);

        await Clients.Group(room.RoomCode).SendAsync("TrackPlayRequested", trackId, room.Position, room.IsPlaying);
    }
    
    public async Task AddTrackToQuery(Guid trackId)
    {
        var userId = GetUserId(Context);
        
        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        var track = await _tracksService.Get(trackId);

        if (!room.TracksQuery.Contains(trackId))
        {
            room.TracksQuery.Add(trackId);
            await _roomsRepository.UpdateAsync(room);
        }

        await Clients.Group(room.RoomCode).SendAsync("TrackAddedToQuery", track);
    }

    public async Task RemoveTrackFromQuery(Guid trackId)
    {
        var userId = GetUserId(Context);
        
        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");
        
        if (room.TracksQuery.Contains(trackId))
        {
            room.TracksQuery.Remove(trackId);
            await _roomsRepository.UpdateAsync(room);
        }
        await Clients.Group(room.RoomCode).SendAsync("TrackRemovedFromQuery", trackId);
    }

    public async Task UpdateQuery()
    {
        var userId = GetUserId(Context);
        
        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) throw new NotFoundException("Room not found");

        var tracks = await _tracksService.GetAll(room.TracksQuery);
        await Clients.Group(room.RoomCode).SendAsync("QueryFullUpdate", tracks);
    }
    
    public async Task<RoomDto> CreateRoom()
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room != null) await RemoveFromRoom(room.RoomCode);

        room = new RoomBase
        {
            CreationDate = DateTimeOffset.UtcNow,
            OwnerId = userId,
            RoomCode = StringHelpers.RandomString(8),
            ContainerType = ContainerType.Room,
            IsPlaying = false,
            Position = 0,
            CurrentContainerId = Guid.Empty,
            CurrentTrackId = Guid.Empty,
            PositionUpdateTime = DateTimeOffset.UtcNow
        };
        await _roomsRepository.CreateAsync(room);

        var curRoom = await AddToRoom(room.RoomCode);
        return await ToDto(curRoom!);
    }
    
    private readonly IRepository<RoomBase> _roomsRepository;
    private readonly IRepository<RoomUser> _roomUsersRepository;
    private readonly ServiceSettings _settings;
    private readonly ITracksService _tracksService;
    private readonly IRepository<UserMongoBase> _usersRepository;

    public StreamingHub(IRepository<RoomBase> roomsRepository, IRepository<UserMongoBase> usersRepository,
        IRepository<RoomUser> roomUsersRepository, ServiceSettings settings, ITracksService tracksService)
    {
        _roomsRepository = roomsRepository;
        _usersRepository = usersRepository;
        _roomUsersRepository = roomUsersRepository;
        _settings = settings;
        _tracksService = tracksService;
    }

    private static Guid GetUserId(HubCallerContext context)
    {
        return Guid.Parse(context.User!.Claims.First(i => i.Type == "UserId").Value);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId(Context);
        var user = await _roomUsersRepository.GetAsync(x => x.Id == userId);
        if (user == null)
        {
            user = new RoomUser
            {
                Id = userId
            };
            user.ConnectionIds.Add(Context.ConnectionId);
            await _roomUsersRepository.CreateAsync(user);
        }
        else
        {
            if (!user.ConnectionIds.Contains(Context.ConnectionId))
            {
                user.ConnectionIds.Add(Context.ConnectionId);
                await _roomUsersRepository.UpdateAsync(user);
            }

            var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
            if (room != null) await AddToRoom(room.RoomCode);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId(Context);
        var user = await _roomUsersRepository.GetAsync(x => x.Id == userId);
        if (user == null) return;

        if (user.ConnectionIds.Contains(Context.ConnectionId))
        {
            user.ConnectionIds.Remove(Context.ConnectionId);
            await _roomUsersRepository.UpdateAsync(user);
        }

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room != null) await RemoveFromRoom(room.RoomCode);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task LeaveRoom()
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId));
        if (room == null) return;

        await RemoveFromRoom(room.RoomCode);
    }
    private async Task<RoomDto> ToDto(RoomBase room)
    {
        var users = await _usersRepository.GetAllAsync(room.RoomUserIds);
        var userDtos = users.Select(ToDto).ToArray();
        var trackDtos = await _tracksService.GetAll(room.TracksQuery);
        return room.AsDto(userDtos, trackDtos);
    }
    private UserShortDto ToDto(IUserBase userMongoBase)
    {
        return userMongoBase.AsShortDto(new ImagePath("Users", _settings.GatewayPath));
    }

    public async Task<RoomDto> JoinCurrentRoom()
    {
        var userId = GetUserId(Context);
        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId))
                   ?? throw new NotFoundException("Room not found");

        var curRoom = await AddToRoom(room.RoomCode)
                      ?? throw new NotFoundException("Room not found");

        return await ToDto(curRoom);
    }

    public async Task<RoomDto> JoinRoom(string roomCode)
    {
        var room = await AddToRoom(roomCode)
                   ?? throw new NotFoundException("Room not found");

        return await ToDto(room);
    }

    public async Task KickUser(Guid userIdToRemove)
    {
        var userId = GetUserId(Context);

        var room = await _roomsRepository.GetAsync(x => x.RoomUserIds.Contains(userId))
                   ?? throw new NotFoundException("Room not found");

        if (room.OwnerId != userId) throw new ForbiddenException("Only room owner can kick users");
        if(userId == userIdToRemove) throw new ForbiddenException("Can't kick yourself");

        await Clients.Group(room.RoomCode).SendAsync("UserKicked", userIdToRemove);
        await RemoveFromRoom(room.RoomCode, userIdToRemove);
    }
    public async Task<RoomBase?> AddToRoom(string roomCode, Guid? userToAdd = null)
    {
        var userId = userToAdd ?? GetUserId(Context);
        var user = await _usersRepository.GetAsync(userId);
        if (user == null) return null;
        var room = await _roomsRepository.GetAsync(x => x.RoomCode == roomCode.ToUpper());
        if (room == null) return null;

        if (!room.RoomUserIds.Contains(userId))
        {
            room.RoomUserIds.Add(userId);
            await _roomsRepository.UpdateAsync(room);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        await Clients.Group(roomCode).SendAsync("UserConnected", ToDto(user));

        return room;
    }
    public async Task RemoveFromRoom(string roomCode, Guid? userToRemove = null)
    {
        var userId = userToRemove ?? GetUserId(Context);
        var room = await _roomsRepository.GetAsync(x => x.RoomCode == roomCode.ToUpper());
        if (room == null) return;

        room.RoomUserIds.Remove(userId);
        if (room.RoomUserIds.Count == 0 || room.OwnerId == userId)
        {
            await _roomsRepository.RemoveAsync(room.Id);
            await Clients.Group(room.RoomCode).SendAsync("RoomDeleted");
            return;
        }

        await _roomsRepository.UpdateAsync(room);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
        await Clients.Group(roomCode).SendAsync("UserDisconnected", userId);
    }
}