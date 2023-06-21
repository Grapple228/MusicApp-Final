using System.Windows;
using System.Windows.Media;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Shared.DTOs.Streaming;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class ArtistPopupViewModel : PopupNavigationViewModelBase
{
    public ArtistPopupViewModel(Guid mediaId, NavigationPopupOptions options, bool isCreate)
    {
        Navigations.Add(new PopupNavigationModel(
            "Add to playlist",
            (PathGeometry)Application.Current.Resources["AddToPlaylistIcon"],
            () =>
            {
                DialogViewModel.OpenGlobal("Add to playlist",
                    new AddToPlaylistViewModel(mediaId, AddToPlaylist.Artist));
            }));

        if (options.IsAdmin || options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Image",
                (PathGeometry)Application.Current.Resources["ImageIcon"],
                () => { Task.Run(() => App.GetService().ChangeImage(mediaId, ChangeImage.Artists)); }));

        if (isCreate || options.IsAdmin || options.IsOwner)
        {
            if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
                Navigations.Add(new PopupNavigationModel(
                    "Create track",
                    (PathGeometry)Application.Current.Resources["AddIcon"],
                    () =>
                    {
                        DialogViewModel.OpenGlobal("Create track",
                            new CreateTrackViewModel(mediaId, ApplicationService.IsUserArtist()));
                    }));

            if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
                Navigations.Add(new PopupNavigationModel(
                    "Create album",
                    (PathGeometry)Application.Current.Resources["AddIcon"],
                    () =>
                    {
                        DialogViewModel.OpenGlobal("Create album",
                            new CreateAlbumViewModel(mediaId, ApplicationService.IsUserArtist()));
                    }));
        }
        else
        {
            if (options.IsActionsAllowed && (options.IsAdmin || options.IsArtist || options.IsOwner))
                Navigations.Add(new PopupNavigationModel(
                    "Add track",
                    (PathGeometry)Application.Current.Resources["AddIcon"],
                    () =>
                    {
                        DialogViewModel.OpenGlobal("Add track", new AddTrackViewModel(mediaId, ContainerType.Artist));
                    }));

            if (options.IsActionsAllowed && (options.IsAdmin || options.IsArtist || options.IsOwner))
                Navigations.Add(new PopupNavigationModel(
                    "Add to album",
                    (PathGeometry)Application.Current.Resources["AddIcon"],
                    () =>
                    {
                        DialogViewModel.OpenGlobal("Add to album",
                            new AddToAlbumViewModel(mediaId, ContainerType.Artist));
                    }));
        }

        switch (options.RemoveType)
        {
            case RemoveType.Remove when options.IsAdmin || options.IsOwner || options.IsSecondaryOwner ||
                                        options.IsContainerOwner:
                if (options.IsAdmin || options.IsOwner || options.IsContainerOwner)
                    if (options.IsRemovable)
                        Navigations.Add(new PopupNavigationModel(
                            "Remove",
                            (PathGeometry)Application.Current.Resources["TrashIcon"],
                            () => { ArtistEvents.RequestRemove(mediaId, options.ContainerType); },
                            true));
                break;
            case RemoveType.Delete:
            case RemoveType.Nothing:
            default:
                break;
        }
    }

    public override string ModelName { get; protected set; } = nameof(ArtistPopupViewModel);
}