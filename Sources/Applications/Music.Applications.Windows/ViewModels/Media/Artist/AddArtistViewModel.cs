using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Artist;

public class AddArtistViewModel : LoadableViewModel
{
    private readonly ContainerType _containerType;
    private readonly Guid _mediaId;

    public AddArtistViewModel(Guid mediaId, ContainerType containerType)
    {
        _mediaId = mediaId;
        _containerType = containerType;
        Artists = new ObservableCollection<Models.Media.Artists.Artist>();
        Task.Run(async () => await LoadInfo(Guid.Empty));
        ArtistEvents.RequestedAddArtist += ArtistEventsOnRequestedAddArtist;
    }

    public ObservableCollection<Models.Media.Artists.Artist> Artists { get; }

    public override string ModelName { get; protected set; } = nameof(AddArtistViewModel);

    private void ArtistEventsOnRequestedAddArtist(Guid artistId)
    {
        ArtistEvents.ArtistAdd(artistId, _mediaId, _containerType);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var userId = ApplicationService.CurrentUserId();
        var artists = (await App.GetService().GetArtists()).Where(x => x.Id != userId);

        ApplicationService.Invoke(() => Artists.Clear());
        foreach (var artistDto in artists)
            ApplicationService.Invoke(() => Artists.Add(new Models.Media.Artists.Artist(artistDto)));
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    ~AddArtistViewModel()
    {
        Dispose();
    }


    public override void Dispose()
    {
        ArtistEvents.RequestedAddArtist -= ArtistEventsOnRequestedAddArtist;
        base.Dispose();
    }
}