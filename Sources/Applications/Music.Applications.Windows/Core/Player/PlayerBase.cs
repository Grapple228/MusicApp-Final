using System.ComponentModel;
using System.Diagnostics;
using Audio.Other;
using Audio.Wave;
using Audio.Wave.WaveOutputs;
using Audio.Wave.WaveStreams;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Default;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Core.Player;

public interface IPlayerBase : INotifyPropertyChanged, IDisposable
{
    public PlayerType PlayerType { get; }
    public ShuffleType ShuffleType { get; set; }
    public RepeatType RepeatType { get; set; }
    public VolumeType VolumeType { get; set; }
    public float Volume { get; set; }
    public long MaximumPosition { get; set; }
    public long CurrentPosition { get; set; }
    public void ChangeMute();
    void ChangePlay();
    void ChangePlay(bool isPlaying);
    void RequestPlay(Guid tId, long position, bool isPlaying);
    void RequestPlay(Guid tId, Guid cId, ContainerType cType);
    event EndOfTrackHandler? EndOfTrack;
    void Clear();
    bool ChangePosition(long position);
    WaveStream WaveStream { get; }
}

public delegate void EndOfTrackHandler();

public abstract class PlayerBase : ObservableObject, IPlayerBase
{
    protected readonly WaveOutEvent waveout = new();
    protected Guid containerId;
    protected ContainerType containerType;
    protected long currentPosition;
    protected long maximumPosition;
    protected RepeatType repeatType;
    protected ShuffleType shuffleType;
    protected Timer? timer;
    protected Guid trackId;
    protected float volumeBeforeMuting;
    protected VolumeType volumeType;

    protected PlayerBase(SettingsViewModel settings)
    {
        Volume = settings.Volume;
        PlayerEvents.ClearRequested += PlayerEventsOnClearRequested;
       
    }

    protected Guid TrackId
    {
        get => trackId;
        set
        {
            if (value.Equals(trackId)) return;
            trackId = value;
            OnPropertyChanged();
        }
    }

    protected Guid ContainerId
    {
        get => containerId;
        set
        {
            if (value.Equals(containerId)) return;
            containerId = value;
            OnPropertyChanged();
        }
    }

    protected ContainerType ContainerType
    {
        get => containerType;
        set
        {
            if (value == containerType) return;
            containerType = value;
            OnPropertyChanged();
        }
    }

    private bool IsMuted => Volume == 0;

    public WaveStream? WaveStream { get; private set; }

    public bool IsPlaying => waveout.PlaybackState == PlaybackState.Playing;
    public event EndOfTrackHandler? EndOfTrack;
    public void Clear()
    {
        CurrentPosition = 0;
        MaximumPosition = 1;
        ChangePlay(false);
        WaveStream?.Dispose();
        WaveStream = null;
        TrackId = Guid.Empty;
        ContainerType = ContainerType.None;
        ContainerId = Guid.Empty;
    }

    public long CurrentPosition
    {
        get => currentPosition;
        set
        {
            if (WaveStream == null) return;
            if(TrackId == Guid.Empty) return;
            var pos = GetAlignedPosition(value, WaveStream.Length, WaveStream.BlockAlign);
            if (!ChangePosition(pos)) return;
            OnPropertyChanged();
            ChangePlay(true);
            var control = App.ServiceProvider.GetRequiredService<RoomViewModel>();
            Task.Run(() => control.ChangePosition(pos));
        }
    }

    public abstract PlayerType PlayerType { get; }

    public ShuffleType ShuffleType
    {
        get => shuffleType;
        set
        {
            if (value == shuffleType) return;
            shuffleType = value;
            OnPropertyChanged();
        }
    }

    public RepeatType RepeatType
    {
        get => repeatType;
        set
        {
            if (value == repeatType) return;
            repeatType = value;
            OnPropertyChanged();
        }
    }

    public VolumeType VolumeType
    {
        get => volumeType;
        set
        {
            if (value == volumeType) return;
            volumeType = value;
            OnPropertyChanged();
        }
    }

    public float Volume
    {
        get => waveout.Volume;
        set
        {
            waveout.Volume = value;
            VolumeType = value switch
            {
                0 => VolumeType.Muted,
                < 0.5f => VolumeType.Low,
                _ => VolumeType.High
            };
            OnPropertyChanged();
        }
    }

    public long MaximumPosition
    {
        get => maximumPosition;
        set
        {
            if (value == maximumPosition) return;
            maximumPosition = value;
            OnPropertyChanged();
        }
    }

    public void ChangeMute()
    {
        if (IsMuted)
        {
            Volume = volumeBeforeMuting == 0.0f ? 0.2f : volumeBeforeMuting;
        }
        else
        {
            volumeBeforeMuting = Volume;
            Volume = 0;
        }
    }

    public void Dispose()
    {
        PlayerEvents.ClearRequested -= PlayerEventsOnClearRequested;
        waveout.Dispose();
        timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ChangePlay()
    {
        if (!IsPlaying) waveout.Play();
        else waveout.Pause();
        PlayerEvents.ChangePlay(TrackId, ContainerId, ContainerType, IsPlaying);
    }

    public void ChangePlay(bool isPlaying)
    {
        if (isPlaying) waveout.Play();
        else waveout.Pause();
        PlayerEvents.ChangePlay(TrackId, ContainerId, ContainerType, isPlaying);
    }

    public async void RequestPlay(Guid tId, long position, bool isPlaying)
    {
        if (tId == Guid.Empty)
        {
            ChangePlay(false);
            TrackId = tId;
            ContainerId = Guid.Empty;
            ContainerType = ContainerType.None;
            return;
        }

        if (TrackId == tId)
        {
            CurrentPosition = position;
            ChangePlay(isPlaying);
        }
        else
        {
            await SetTrack(tId);
            CurrentPosition = position;
            ChangePlay(true);
        }
    }

    public virtual async void RequestPlay(Guid tId, Guid cId, ContainerType cType)
    {
        if ((tId == Guid.Empty) && cType == ContainerType.None)
        {
            ChangePlay(false);
            TrackId = tId;
            ContainerId = cId;
            ContainerType = cType;
            return;
        }
        
        if (ContainerType == cType && ContainerId == cId && TrackId == tId)
        {
            ChangePlay();
        }
        else
        {
            await SetTrack(tId, cId, cType);
            ChangePlay(true);
        }
    }

    private static long GetAlignedPosition(long value, long streamLength, int align)
    {
        return streamLength * (value - value % align) / streamLength;
    }

    public abstract bool ChangePosition(long position);

    private void StartTimer()
    {
        timer ??= new Timer(TimerTick, 0, TimeSpan.Zero, TimeSpan.FromMilliseconds(250));
    }

    protected void TrackEnded()
    {
        EndOfTrack?.Invoke();
    }

    private void TimerTick(object? state)
    {
        if (WaveStream == null) return;
        currentPosition = WaveStream.Position;
        if (WaveStream.Position == WaveStream.Length) TrackEnded();
        OnPropertyChanged(nameof(CurrentPosition));
    }

    protected virtual void PlayerEventsOnClearRequested()
    {
        timer?.Dispose();
        timer = null;
        ChangePlay(false);
        ContainerId = Guid.Empty;
        TrackId = Guid.Empty;
        ContainerType = ContainerType.None;
        WaveStream?.Dispose();
    }

    protected static async Task<BlockAlignReductionStream> GetAudioStream(Guid trackId)
    {
        var stream = await App.GetService().GetStream(trackId);
        var reader = new Mp3FileReader(stream);
        var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
        return new BlockAlignReductionStream(pcmStream);
    }

    public virtual async Task SetTrack(Guid tId, Guid cId, ContainerType cType)
    {
        BlockAlignReductionStream? stream;

        try
        {
            stream = await GetAudioStream(tId);
            StartTimer();
        }
        catch
        {
            NotificationEvents.DisplayNotification("Couldn't load track", "Track problem", NotificationType.Error);
            return;
        }

        WaveStream = stream;
        MaximumPosition = WaveStream.Length;
        waveout.Stop();
        waveout.Init(WaveStream);
        waveout.Play();
        TrackId = tId;
        ContainerId = cId;
        ContainerType = cType;
    }
    
    public virtual async Task SetTrack(Guid tId)
    {
        BlockAlignReductionStream? stream;

        try
        {
            stream = await GetAudioStream(tId);
            StartTimer();
        }
        catch
        {
            NotificationEvents.DisplayNotification("Couldn't load track", "Track problem", NotificationType.Error);
            return;
        }

        WaveStream = stream;
        MaximumPosition = WaveStream.Length;
        waveout.Stop();
        waveout.Init(WaveStream);
        waveout.Play();
        TrackId = tId;
        ContainerId = Guid.Empty;
        ContainerType = ContainerType.None;
    }

    ~PlayerBase()
    {
        Dispose();
    }
}