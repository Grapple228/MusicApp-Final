using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class PlaylistPopupViewModel : PopupNavigationViewModelBase
{
    public PlaylistPopupViewModel(Guid mediaId, NavigationPopupOptions options)
    {
        Navigations.Add(new PopupNavigationModel(
            "Add to playlist",
            (PathGeometry)Application.Current.Resources["AddToPlaylistIcon"],
            () =>
            {
                DialogViewModel.OpenGlobal("Add to playlist",
                    new AddToPlaylistViewModel(mediaId, AddToPlaylist.Playlist));
            }));

        if (options.IsAdmin || options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Image",
                (PathGeometry)Application.Current.Resources["ImageIcon"],
                () => { Task.Run(() => App.GetService().ChangeImage(mediaId, ChangeImage.Playlists)); }));

        switch (options.RemoveType)
        {
            case RemoveType.Remove when options.IsAdmin || options.IsOwner || options.IsSecondaryOwner ||
                                        options.IsContainerOwner:
                if (options.IsAdmin || options.IsOwner || options.IsContainerOwner)
                    Navigations.Add(new PopupNavigationModel(
                        "Remove",
                        (PathGeometry)Application.Current.Resources["TrashIcon"],
                        () => { },
                        true));
                break;
            case RemoveType.Delete when options.IsAdmin || options.IsOwner:
                Navigations.Add(new PopupNavigationModel(
                    "Delete",
                    (PathGeometry)Application.Current.Resources["TrashIcon"],
                    () => { Task.Run(async () => { await App.GetService().DeletePlaylist(mediaId); }); },
                    true));
                break;
            case RemoveType.Nothing:
                break;
        }
    }

    public override string ModelName { get; protected set; } = nameof(PlaylistPopupViewModel);
}