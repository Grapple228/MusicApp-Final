using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Artist;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Shared.DTOs.Streaming;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class AlbumPopupViewModel : PopupNavigationViewModelBase
{
    public AlbumPopupViewModel(Guid mediaId, NavigationPopupOptions options)
    {
        Navigations.Add(new PopupNavigationModel(
            "Add to playlist",
            (PathGeometry)Application.Current.Resources["AddToPlaylistIcon"],
            () =>
            {
                DialogViewModel.OpenGlobal("Add to playlist",
                    new AddToPlaylistViewModel(mediaId, AddToPlaylist.Album));
            }));

        if (options.IsAdmin || options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Image",
                (PathGeometry)Application.Current.Resources["ImageIcon"],
                () => { Task.Run(() => App.GetService().ChangeImage(mediaId, ChangeImage.Albums)); }));


        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner || options.IsSecondaryOwner))
            Navigations.Add(new PopupNavigationModel(
                "Add track",
                (PathGeometry)Application.Current.Resources["AddIcon"],
                () =>
                {
                    DialogViewModel.OpenGlobal("Add track", new AddTrackViewModel(mediaId, ContainerType.Album));
                }));

        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
            Navigations.Add(new PopupNavigationModel(
                "Add artist",
                (PathGeometry)Application.Current.Resources["AddIcon"],
                () =>
                {
                    DialogViewModel.OpenGlobal("Add artist", new AddArtistViewModel(mediaId, ContainerType.Album));
                }));

        switch (options.RemoveType)
        {
            case RemoveType.Remove:
                if ((options.IsAdmin || options.IsContainerOwner) && options.ContainerType == ContainerType.Artist)
                    Navigations.Add(new PopupNavigationModel(
                        "Delete",
                        (PathGeometry)Application.Current.Resources["TrashIcon"],
                        () => { Task.Run(async () => { await App.GetService().DeleteAlbum(mediaId); }); },
                        true));
                else
                    Navigations.Add(new PopupNavigationModel(
                        "Remove",
                        (PathGeometry)Application.Current.Resources["TrashIcon"],
                        () => { AlbumEvents.RequestRemove(mediaId, options.ContainerType); },
                        true));
                break;
            case RemoveType.Delete when options.IsAdmin || options.IsOwner:
                Navigations.Add(new PopupNavigationModel(
                    "Delete",
                    (PathGeometry)Application.Current.Resources["TrashIcon"],
                    () => { Task.Run(async () => { await App.GetService().DeleteAlbum(mediaId); }); },
                    true));
                break;
            case RemoveType.Nothing:
            default:
                break;
        }
    }

    public override string ModelName { get; protected set; } = nameof(AlbumPopupViewModel);
}