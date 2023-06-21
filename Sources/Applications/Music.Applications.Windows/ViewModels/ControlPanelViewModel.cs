using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Core.Player;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels.Default;
using Music.Shared.DTOs.Media.Enums;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;

namespace Music.Applications.Windows.ViewModels;

public class ControlPanelViewModel : LoadableViewModel, IPlayable
{
    private Guid _containerId;
    private ContainerType _containerType;

    private Track? _currentTrack;
    private long _cursorTime;
    private bool _isPlaying;
    private bool _isShown;

    private IPlayerBase? _player;
    private double _timeOffset;
    private Guid _trackId;
    private bool _isShuffleEnabled = true;
    private bool _isRepeatEnabled = true;
    private bool _isReactionEnabled = true;
    private bool _isSliderEnabled = true;

    public bool IsSliderEnabled
    {
        get => _isSliderEnabled;
        set
        {
            if (value == _isSliderEnabled) return;
            _isSliderEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsReactionEnabled
    {
        get => _isReactionEnabled;
        set
        {
            if (value == _isReactionEnabled) return;
            _isReactionEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsShuffleEnabled
    {
        get => _isShuffleEnabled;
        set
        {
            if (value == _isShuffleEnabled) return;
            _isShuffleEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsRepeatEnabled
    {
        get => _isRepeatEnabled;
        set
        {
            if (value == _isRepeatEnabled) return;
            _isRepeatEnabled = value;
            OnPropertyChanged();
        }
    }

    public ControlPanelViewModel(SettingsViewModel settings, RoomViewModel roomViewModel)
    {
        Player = new SinglePlayer(settings);
        RoomViewModel = roomViewModel;
        MediaEvents.TrackInfoChanged += MediaEventsOnTrackInfoChanged;
        GlobalEvents.UpdateRequested += GlobalEventsOnUpdateRequested;

        PlayerEvents.TrackPlayRequested += PlayerEventsOnTrackPlayRequested;
        PlayerEvents.ContainerPlayRequested += PlayerEventsOnContainerPlayRequested;
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
        PlayerEvents.PlayChangeRequested += PlayerEventsOnPlayChangeRequested;
        PlayerEvents.ClearRequested += PlayerEventsOnClearRequested;
    }

    public RoomViewModel RoomViewModel { get; }

    public bool IsShown
    {
        get => _isShown;
        set
        {
            if (value == _isShown) return;
            _isShown = value;
            OnPropertyChanged();
        }
    }

    public double TimeOffset
    {
        get => _timeOffset;
        set
        {
            var val = value - 18;
            if (val.Equals(_timeOffset)) return;
            _timeOffset = val;
            OnPropertyChanged();
        }
    }

    public long CursorTime
    {
        get => _cursorTime;
        set
        {
            if (value.Equals(_cursorTime)) return;
            _cursorTime = value;
            OnPropertyChanged();
        }
    }
    
    public IPlayerBase? Player
    {
        get => _player;
        private set
        {
            if (_player != null) _player.EndOfTrack -= PlayerOnEndOfTrack;
            _player?.Dispose();
            _player = value;
            if (_player != null) _player.EndOfTrack += PlayerOnEndOfTrack;
            OnPropertyChanged();
        }
    }

    public Guid TrackId
    {
        get => _trackId;
        private set
        {
            _trackId = value;
            OnPropertyChanged();
        }
    }

    public ContainerType ContainerType
    {
        get => _containerType;
        private set
        {
            if (value == _containerType) return;
            _containerType = value;
            OnPropertyChanged();
        }
    }

    public Guid ContainerId
    {
        get => _containerId;
        private set
        {
            if (value.Equals(_containerId)) return;
            _containerId = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ArtistShort>? Artists => CurrentTrack?.Artists;

    public Track? CurrentTrack
    {
        get => _currentTrack;
        private set
        {
            var valueIsNull = value == null;
            IsReactionEnabled = !valueIsNull;
            IsSliderEnabled = !valueIsNull;
            if (Equals(value, _currentTrack)) return;
            _currentTrack = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Artists));
            IsShown = !valueIsNull;
        }
    }

    public ObservableCollection<Track> ListenQuery { get; } = new();

    public override string ModelName { get; protected set; } = nameof(ControlPanelViewModel);

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value == _isPlaying) return;
            _isPlaying = value;
            OnPropertyChanged();
        }
    }

    public bool IsCurrent { get; set; }

    public Guid Id { get; init; }

    public async Task NextTrack()
    {
        if(CurrentTrack == null) return;
        
        if (RoomViewModel.IsInRoom)
        {
            if(ApplicationService.IsUserOwner(RoomViewModel.Room!.OwnerId))
                await RoomViewModel.NextTrack();
            return;
        }
        
        switch (Player?.RepeatType)
        {
            case RepeatType.None:
            case RepeatType.Track:
                ProcessTrack();
                return;
            case RepeatType.Container:
                ProcessContainer();
                return;
            default:
                return;
        }

        void ProcessTrack()
        {
            var track = ListenQuery.FirstOrDefault(x => x.Id == CurrentTrack.Id);
            if (track == null) return;
            var index = ListenQuery.IndexOf(track) + 1;
            if (index >= ListenQuery.Count)
            {
                Player.CurrentPosition = 0;
                Player.ChangePlay(false);
                PlayerEvents.ChangePlay(TrackId, ContainerId, ContainerType, false);
                return;
            }

            CurrentTrack = ListenQuery[index];
            TrackId = CurrentTrack.Id;
            Player?.RequestPlay(CurrentTrack.Id, ContainerId, ContainerType);
        }

        void ProcessContainer()
        {
            var track = ListenQuery.FirstOrDefault(x => x.Id == CurrentTrack.Id);
            if (track == null) return;

            if (ListenQuery.Count == 1)
            {
                Player.CurrentPosition = 0;
                return;
            }

            var index = ListenQuery.IndexOf(track) + 1;
            CurrentTrack = index < ListenQuery.Count ? ListenQuery[index] : ListenQuery[0];

            TrackId = CurrentTrack.Id;
            Player?.RequestPlay(CurrentTrack.Id, ContainerId, ContainerType);
        }
    }

    public async Task PrevTrack()
    {
        if (CurrentTrack == null) return;

        if (RoomViewModel.IsInRoom)
        {
            await RoomViewModel.PrevTrack();
            return;
        }

        if (Player is { CurrentPosition: > 800000 })
        {
            Player.CurrentPosition = 0;
            return;
        }

        var track = ListenQuery.FirstOrDefault(x => x.Id == CurrentTrack.Id);
        if (track == null) return;

        var index = ListenQuery.IndexOf(track) - 1;
        if (index < 0) return;
        CurrentTrack = ListenQuery[index];
        TrackId = CurrentTrack.Id;
        Player?.RequestPlay(CurrentTrack.Id, ContainerId, ContainerType);
    }

    private async void PlayerOnEndOfTrack()
    {
        switch (Player?.RepeatType)
        {
            case RepeatType.None:
                await NextTrack();
                return;
            case RepeatType.Container:
                await NextTrack();
                break;
            case RepeatType.Track:
                Player.CurrentPosition = 0;
                PlayerEvents.ChangePlay(TrackId, ContainerId, ContainerType, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PlayerEventsOnPlayChangeRequested()
    {
        if (CurrentTrack == null) return;
        PlayerEvents.RequestTrackPlay(CurrentTrack.Id, ContainerId, ContainerType);
    }

    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        IsPlaying = isPlaying;
        
        var current = ListenQuery.FirstOrDefault(x => x.IsCurrent);
        if (current != null)
        {
            if (current.Id == trackId)
            {
                current.IsPlaying = isPlaying;
                return;
            }

            current.IsCurrent = false;
            current.IsPlaying = false;
        }


        var track = ListenQuery.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;

        track.IsCurrent = true;
        track.IsPlaying = isPlaying;
    }

    public void AddTrackToQuery(TrackDto track)
    {
        var existing = ListenQuery.FirstOrDefault(x => x.Id == track.Id);
        if(existing != null) return;
        ApplicationService.Invoke(() => ListenQuery.Add(new Track(track)));
    }
    
    public void RemoveTrackFromQuery(Guid trackId)
    {
        var existing = ListenQuery.FirstOrDefault(x => x.Id == trackId);
        if(existing == null) return;
        ApplicationService.Invoke(() => ListenQuery.Remove(existing));
    }
    
    public void UpdateQuery(IEnumerable<Track> tracks)
    {
        ApplicationService.Invoke(() => ListenQuery.Clear());
        foreach (var track in tracks)
        {
            ApplicationService.Invoke(() => ListenQuery.Add(track));
        }
    }

    public async Task SetTrack(Guid trackId, long position, bool isPlaying)
    {
        var service = App.GetService();
        MediaTrackDto track;
        if (CurrentTrack?.Id == trackId)
        {
            Player?.RequestPlay(trackId, position, isPlaying);
        }
        else
        {
            track = await service.GetMediaTrack(trackId);
            CurrentTrack = new Track(track);
            TrackId = CurrentTrack.Id;
            Player?.RequestPlay(trackId, position, true);
        }
    }
    
    private async Task GroupTrackPlayRequested(Guid trackId)
    {
        var track = ListenQuery.FirstOrDefault(x => x.Id == trackId);
        if (track == null)
        {
            NotificationEvents.DisplayNotification("Firstly add track to query", "Track error", NotificationType.Error);
            return;
        }
        
        if(_isLoading) await RoomViewModel.RequestTrackPlay(trackId, RoomViewModel.Room!.Position);
        else await RoomViewModel.RequestTrackPlay(trackId, Player!.CurrentPosition);
    }
    
    private async Task GroupContainerPlayRequested(Guid trackId)
    {
        NotificationEvents.DisplayNotification("Containers are not playable", "Play error", NotificationType.Error);
    }
    
    private async void PlayerEventsOnTrackPlayRequested(Guid trackId, Guid containerId, ContainerType containerType)
    {
        if (trackId == Guid.Empty)
        {
            ClearPanel();
            return;
        }

        if (RoomViewModel.IsInRoom)
        {
            await GroupTrackPlayRequested(trackId);
            return;
        }

        Player?.RequestPlay(trackId, containerId, containerType);
        await SetTrack(trackId, containerId, containerType);
    }

    private async void PlayerEventsOnContainerPlayRequested(Guid containerId, ContainerType containerType)
    {
        if (RoomViewModel.IsInRoom)
        {
            if (containerType == ContainerType.Track)
            {
                await GroupTrackPlayRequested(containerId);
            }
            else if (containerType == ContainerType.Liked)
            {
                await GroupTrackPlayRequested(containerId);
            }
            else if (containerType == ContainerType.Tracks)
            {
                await GroupTrackPlayRequested(containerId);
            }
            else
            {
                await GroupContainerPlayRequested(containerId);
            }
            return;
        }
        
        if (CurrentTrack?.Id == containerId)
        {
            Player?.RequestPlay(TrackId, ContainerId, ContainerType);
            return;
        }

        if (ContainerType == containerType && ContainerId == containerId)
        {
            Player?.RequestPlay(TrackId, ContainerId, ContainerType);
            return;
        }

        switch (containerType)
        {
            case ContainerType.Track:
                await ProcessTrack();
                break;
            case ContainerType.Liked:
            case ContainerType.Tracks:
            case ContainerType.Playlist:
            case ContainerType.Album:
            case ContainerType.Artist:
                await ProcessContainer();
                break;
            default:
                return;
        }

        TrackId = CurrentTrack!.Id;
        Player?.RequestPlay(TrackId, ContainerId, ContainerType);

        async Task ProcessTrack()
        {
            var track = await App.GetService().GetMediaTrack(containerId);
            CurrentTrack = new Track(track);
            ContainerId = CurrentTrack.Id;
            ContainerType = ContainerType.Track;

            ApplicationService.Invoke(() => ListenQuery.Clear());
            ApplicationService.Invoke(() => ListenQuery.Add(CurrentTrack));
        }

        async Task ProcessContainer()
        {
            IReadOnlyCollection<MediaTrackDto> tracks;
            Guid newContainerId;

            Func<MediaTrackDto, bool>? filter;
            switch (containerType)
            {
                case ContainerType.Playlist:
                case ContainerType.Album:
                case ContainerType.Artist:
                    tracks = await App.GetService().GetContainerTracks(containerId, containerType);
                    filter = null;
                    newContainerId = containerId;
                    break;
                case ContainerType.Liked:
                    tracks = await App.GetService().GetMediaTracks(MediaFilterEnum.Liked);
                    filter = x => x.Id == containerId;
                    newContainerId = TrackId;
                    break;
                case ContainerType.Tracks:
                    tracks = await App.GetService().GetMediaTracks();
                    filter = x => x.Id == containerId;
                    newContainerId = TrackId;
                    break;
                case ContainerType.User:
                case ContainerType.Track:
                case ContainerType.None:
                default: return;
            }

            var track = filter == null
                ? tracks.FirstOrDefault()
                : tracks.FirstOrDefault(filter);
            if (track == null) return;

            CurrentTrack = new Track(track);
            ContainerType = containerType;
            ContainerId = newContainerId;

            ApplicationService.Invoke(() => ListenQuery.Clear());
            foreach (var mediaTrackDto in tracks)
                ApplicationService.Invoke(() => ListenQuery.Add(new Track(mediaTrackDto)));
        }
    }

    public void RemoveRoom()
    {
        IsShuffleEnabled = true;
        IsRepeatEnabled = true;
        IsReactionEnabled = CurrentTrack != null;
    }

    private bool _isLoading = false;
    
    public void LoadRoom(Room room)
    {
        TrackId = room.CurrentTrackId;
        UpdateQuery(room.Tracks);

        IsShuffleEnabled = false;
        IsRepeatEnabled = false;

        _isLoading = true;
        PlayerEvents.RequestTrackPlay(TrackId, ContainerId, ContainerType);
        _isLoading = false;
        
        IsReactionEnabled = CurrentTrack != null;
        Player!.ShuffleType = ShuffleType.None;
        Player!.RepeatType = RepeatType.None;
        IsSliderEnabled = CurrentTrack != null;
        IsShown = true;
    }
    
    public void ClearPanel()
    {
        TrackId = Guid.Empty;
        ContainerId = Guid.Empty;
        ContainerType = ContainerType.None;
        ApplicationService.Invoke(() => CurrentTrack?.Artists.Clear());
        _currentTrack = null;
        OnPropertyChanged(nameof(CurrentTrack));
        ApplicationService.Invoke(() => ListenQuery.Clear());
        Player?.Clear();
        PlayerEvents.ChangePlay(TrackId, ContainerId, ContainerType, false);
    }
    
    private async Task SetTrack(Guid trackId, Guid containerId, ContainerType containerType)
    {
        if (RoomViewModel.IsInRoom)
        {
            await LoadInfo(trackId);
            return;
        }
        
        if (TrackId == trackId && ContainerId == containerId && ContainerType == containerType) return;

        TrackId = trackId;
        ContainerId = containerId;
        ContainerType = containerType;

        await LoadInfo(trackId);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        if (RoomViewModel.IsInRoom)
        {
            await LoadTrack();
        }
        else
        {
            await LoadTrack();
            await LoadListenQuery();
        }

        async Task LoadTrack()
        {
            if (TrackId == Guid.Empty) return;
            try
            {
                var track = await App.GetService().GetMediaTrack(TrackId, true);
                ApplicationService.Invoke(() => CurrentTrack = new Track(track));
            }
            catch
            {
                // ignored
            }
        }

        async Task LoadListenQuery()
        {
            if (ContainerId == Guid.Empty || ContainerType == ContainerType.None)
            {
                ApplicationService.Invoke(() => ListenQuery.Clear());
                return;
            }

            try
            {
                var tracks = await App.GetService().GetContainerTracks(ContainerId, ContainerType);
                ApplicationService.Invoke(() => ListenQuery.Clear());
                foreach (var track in tracks) ApplicationService.Invoke(() => ListenQuery.Add(new Track(track)));
            }
            catch
            {
                // ignored
            }
        }
    }

    private async void GlobalEventsOnUpdateRequested()
    {
        await Reload();
    }

    private void MediaEventsOnTrackInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        if (CurrentTrack?.Id != id) return;
        CurrentTrack.IsLiked = isLiked;
        CurrentTrack.IsBlocked = isBlocked;
    }

    public override async Task Reload()
    {
        await LoadInfo(TrackId);
    }

    ~ControlPanelViewModel()
    {
        Dispose();
    }

    public override void Dispose()
    {
        MediaEvents.TrackInfoChanged -= MediaEventsOnTrackInfoChanged;
        GlobalEvents.UpdateRequested -= GlobalEventsOnUpdateRequested;

        PlayerEvents.TrackPlayRequested -= PlayerEventsOnTrackPlayRequested;
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;
        PlayerEvents.PlayChangeRequested -= PlayerEventsOnPlayChangeRequested;
        PlayerEvents.ClearRequested -= PlayerEventsOnClearRequested;
        PlayerEvents.ContainerPlayRequested -= PlayerEventsOnContainerPlayRequested;

        RoomViewModel.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    private async void PlayerEventsOnClearRequested()
    {
        IsShown = false;
        CurrentTrack = null;
        TrackId = Guid.Empty;
        ContainerType = ContainerType.None;
        ContainerId = Guid.Empty;
        ListenQuery.Clear();
        await RoomViewModel.Disconnect();
    }
}