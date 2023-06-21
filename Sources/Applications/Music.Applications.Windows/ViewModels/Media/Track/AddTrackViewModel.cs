using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Track;

public class AddTrackViewModel : LoadableViewModel
{
    private readonly ContainerType _containerType;

    private readonly Guid _mediaId;

    public AddTrackViewModel(Guid mediaId, ContainerType containerType)
    {
        _mediaId = mediaId;
        _containerType = containerType;
        Task.Run(async () => await LoadInfo(Guid.Empty));
        TrackEvents.RequestTrackAdded += TrackEventsOnRequestTrackAdded;
    }

    public override string ModelName { get; protected set; } = nameof(AddTrackViewModel);
    public ObservableCollection<Models.Media.Tracks.Track> Tracks { get; } = new();

    private void TrackEventsOnRequestTrackAdded(Guid trackId)
    {
        TrackEvents.AddTrack(trackId, _mediaId, _containerType);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var appService = App.GetService();
        var tracks = ApplicationService.IsUserArtist()
            ? (await appService.GetUserTracks()).Where(x => x.Owner.Id == ApplicationService.CurrentUserId())
            : await appService.GetTracks();

        ApplicationService.Invoke(() => Tracks.Clear());
        foreach (var trackDto in tracks) ApplicationService.Invoke(() => Tracks.Add(new Models.Media.Tracks.Track(trackDto)));
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    ~AddTrackViewModel()
    {
        Dispose();
    }


    public override void Dispose()
    {
        TrackEvents.RequestTrackAdded -= TrackEventsOnRequestTrackAdded;
        base.Dispose();
    }
}