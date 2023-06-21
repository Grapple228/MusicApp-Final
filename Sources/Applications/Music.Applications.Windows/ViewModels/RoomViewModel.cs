using System.Configuration;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Models.Media.Users;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;
using Music.Shared.DTOs.Users;
using Xceed.Wpf.Toolkit;

namespace Music.Applications.Windows.ViewModels;

public class CreateRoomRequest
{
    
}

public class RoomViewModel : LoadableViewModel
{
    public Room? Room
    {
        get => _room;
        set
        {
            IsInRoom = value != null;
            if (Equals(value, _room)) return;
            _room = value;
            OnPropertyChanged();

            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            
            if (IsInRoom)
            {
                control.LoadRoom(value);
            }
            else
            {
                control.RemoveRoom();
            }
        }
    }

    private bool _isInRoom;

    public bool IsInRoom
    {
        get => _isInRoom;
        set
        {
            if (value == _isInRoom) return;
            _isInRoom = value;
            OnPropertyChanged();
        }
    }

    private readonly HubConnection _connection;
    private Room? _room;
    
    private static Task<string?> GetToken()
    {
        var service = App.GetService();
        return Task.FromResult(service.GetToken())!;
    }
    
    public RoomViewModel()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:8000/api/streamingHub", options =>
            {
                options.AccessTokenProvider = GetToken;
                options.Transports = HttpTransportType.WebSockets;
            })
            .Build();

        _connection.On("UserConnected", (UserShortDto user) =>
        {
            if(Room == null) return;
            var existing = Room.Users.FirstOrDefault(x => x.Id == user.Id);
            if(existing != null) return;
            ApplicationService.Invoke(() => Room?.Users.Add(new User(user)));
        });
        
        _connection.On("UserDisconnected", (Guid userId) =>
        {
            var user = Room?.Users.FirstOrDefault(x => x.Id == userId);
            if(user == null) return;
            ApplicationService.Invoke(() => Room?.Users.Remove(user));
        });
        
        _connection.On("RoomDeleted", () =>
        {
            Room = null;
        });
        
        _connection.On("UserKicked", (Guid kickedId) =>
        {
            if(Room == null) return;
            if (ApplicationService.IsUserOwner(kickedId))
            {
                Room = null;
            }
            else
            {
                var user = Room.Users.FirstOrDefault(x => x.Id == kickedId);
                if(user == null) return;
                ApplicationService.Invoke(() => Room.Users.Remove(user));
            }
        });

        _connection.On("TrackAddedToQuery", (TrackDto track) =>
        {
            if(Room == null) return;
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            control.AddTrackToQuery(track);
        });
        
        _connection.On("TrackRemovedFromQuery", (Guid trackId) =>
        {
            if(Room == null) return;
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            control.RemoveTrackFromQuery(trackId);
        });
        
        _connection.On("QueryFullUpdate", (IEnumerable<TrackDto> tracks) =>
        {
            if(Room == null) return;
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            control.UpdateQuery(tracks.Select(x => new Track(x)));
        });
        
        _connection.On("TrackPlayRequested", (Guid trackId, long position, bool isPlaying) =>
        {
            if(Room == null) return;
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            Task.Run(async () => await control.SetTrack(trackId, position, isPlaying));
        });
        
        _connection.On("PositionChanged", (long position) =>
        {
            if(Room == null) return;
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            if(control.Player != null)
                control.Player.ChangePosition(position);
        });

        Task.Run(Connect);
    }

    public async Task NextTrack()
    {
        try
        {
            await _connection.InvokeAsync("NextTrack");
        }
        catch
        {
            // ignored
        }
    }
    
    public async Task PrevTrack()
    {
        try
        {
            await _connection.InvokeAsync("PrevTrack");
        }
        catch
        {
            // ignored
        }
    }

    public async Task ChangePosition(long position)
    {
        try
        {
            await _connection.InvokeAsync("RequestPositionChange", position);
        }
        catch
        {
            // ignored
        }
    }
    
    public async Task Connect()
    {
        try
        {
            await _connection.StartAsync();
            await TryConnectToRoom();
        }
        catch
        {
            // ignored
        }
    }

    public async Task AddTrackToQuery(Guid trackId)
    {
        try
        {
            await _connection.InvokeAsync<TrackDto>("AddTrackToQuery", trackId);
        }
        catch
        {
            // ignored
        }
    }
    
    public async Task RemoveTrackToQuery(Guid trackId)
    {
        try
        {
            await _connection.InvokeAsync<TrackDto>("RemoveTrackFromQuery", trackId);
        }
        catch
        {
            // ignored
        }
    }
    
    public async Task Disconnect()
    {
        try
        {
            await LeaveRoom();
            await _connection.StopAsync();
        }
        catch
        {
            // ignored
        }
        Room = null;
    }

    public async Task RequestTrackPlay(Guid trackId, long position)
    {
        try
        {
            await _connection.InvokeAsync("RequestTrackPlay", trackId, position);
        }
        catch 
        {
            // ignored
        }
    }
    
    public async Task RequestContainerPlay(Guid containerId, ContainerType containerType)
    {
        try
        {
            await _connection.InvokeAsync("RequestContainerPlay", containerId, containerType);
        }
        catch 
        {
            // ignored
        }
    }
    
    public async Task<bool> TryConnectToRoom(string? roomCode = null)
    {
        try
        {
            var roomDto = roomCode == null 
                ? await _connection.InvokeAsync<RoomDto>("JoinCurrentRoom")
                : await _connection.InvokeAsync<RoomDto>("JoinRoom", roomCode);
            Room = new Room(roomDto);
            return true;
        }
        catch
        {
            Room = null;
            return false;
        }
    }

    public async Task KickUser(Guid userId)
    {
        try
        {
            await _connection.InvokeAsync("KickUser", userId);
        }
        catch
        {
            // ignored
        }
    }
    
    public async Task CreateRoom()
    {
        try
        {
            var room = await _connection.InvokeAsync<RoomDto>("CreateRoom");
            Room = new Room(room);
        }
        catch
        {
            Room = null;
        }
    }

    public async Task LeaveRoom()
    {
        try
        {
            await _connection.InvokeAsync("LeaveRoom");
        }
        catch
        {
            // ignored
        }
        finally
        {
            Room = null;
        }
    }
    
    public override string ModelName { get; protected set; } = nameof(RoomViewModel);
    protected override async Task LoadMedia(Guid mediaId)
    {
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    public override async void Dispose()
    {
        await Disconnect();
        GC.SuppressFinalize(this);
    }

    ~RoomViewModel()
    {
        Dispose();
    }
}