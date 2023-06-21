using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Media.Album;

public class AddToAlbumViewModel : LoadableViewModel
{
    private readonly ContainerType _containerType;
    private readonly Guid _mediaId;

    public AddToAlbumViewModel(Guid mediaId, ContainerType containerType)
    {
        _mediaId = mediaId;
        _containerType = containerType;
        Task.Run(async () => await LoadInfo(Guid.Empty));
        AlbumEvents.RequestAddedToAlbum += AlbumEventsOnRequestAddedToAlbum;
    }

    public ObservableCollection<Models.Media.Albums.Album> Albums { get; } = new();

    public override string ModelName { get; protected set; } = nameof(AddToAlbumViewModel);

    private void AlbumEventsOnRequestAddedToAlbum(Guid albumId)
    {
        AlbumEvents.AddToAlbum(albumId, _mediaId, _containerType);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var appService = App.GetService();
        var albums = ApplicationService.IsUserArtist()
            ? (await appService.GetUserAlbums()).Where(x => x.Owner.Id == ApplicationService.CurrentUserId())
            : await appService.GetAlbums();

        ApplicationService.Invoke(() => Albums.Clear());
        foreach (var albumDto in albums)
            ApplicationService.Invoke(() => Albums.Add(new Models.Media.Albums.Album(albumDto)));
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    ~AddToAlbumViewModel()
    {
        Dispose();
    }


    public override void Dispose()
    {
        AlbumEvents.RequestAddedToAlbum -= AlbumEventsOnRequestAddedToAlbum;
        base.Dispose();
    }
}