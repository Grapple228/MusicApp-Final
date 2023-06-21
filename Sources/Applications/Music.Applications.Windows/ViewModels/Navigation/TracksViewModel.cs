using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class TracksViewModel : LoadableViewModel
{
    public TracksViewModel()
    {
        Task.Run(() => LoadInfo(Guid.Empty));
        MediaEvents.TrackInfoChanged += OnTrackInfoChanged;
        TrackEvents.TrackDeleted += TrackEventsOnTrackDeleted;
        
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public ObservableCollection<Track> Tracks { get; private set; }

    public override string ModelName { get; protected set; } = "Tracks";

    private void TrackEventsOnTrackDeleted(Guid trackId)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null) return;
        ApplicationService.Invoke(() => Tracks.Remove(track));
    }

    public override void Dispose()
    {
        MediaEvents.TrackInfoChanged -= OnTrackInfoChanged;
        TrackEvents.TrackDeleted -= TrackEventsOnTrackDeleted;

        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;
        base.Dispose();
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var tracks = await App.GetService().GetUserMediaTracks();
        var trackModels = new ObservableCollection<Track>();
        foreach (var track in tracks) trackModels.Add(new Track(track));
        Tracks = trackModels;
        OnPropertyChanged(nameof(Tracks));
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    private void OnTrackInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var track = Tracks.FirstOrDefault(x => x.Id == id);
        if (track == null) return;

        track.IsLiked = isLiked;
        track.IsBlocked = isBlocked;
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        if (CurrentTrack != null)
        {
            CurrentTrack.IsPlaying = false;
            CurrentTrack.IsCurrent = false;
            CurrentTrack = null;
        }
        
        var track = Tracks.FirstOrDefault(x => x.Id == trackId);
        if(track == null) return;

        CurrentTrack = track;
        CurrentTrack.IsCurrent = true;
        CurrentTrack.IsPlaying = isPlaying;
    }
    
    private ITrack? _currentTrack;
    
    public ITrack? CurrentTrack
    {
        get => _currentTrack;
        set
        {
            if (Equals(value, _currentTrack)) return;
            _currentTrack = value;
            OnPropertyChanged();
        }
    }
}