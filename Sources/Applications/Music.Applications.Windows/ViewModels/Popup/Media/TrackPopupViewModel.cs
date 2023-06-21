using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Applications.Windows.ViewModels.Media.Artist;
using Music.Applications.Windows.ViewModels.Media.Genre;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Shared.DTOs.Streaming;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.ViewModels.Popup.Media;

public class TrackPopupViewModel : PopupNavigationViewModelBase
{
    public TrackPopupViewModel(ITrack track, NavigationPopupOptions options)
    {
        Navigations.Add(new PopupNavigationModel(
            "Add to query",
            (PathGeometry)Application.Current.Resources["AddToPlaylistIcon"],
            () =>
            {
                var room = App.ServiceProvider.GetRequiredService<RoomViewModel>();
                Task.Run(async () => await room.AddTrackToQuery(track.Id));
            }));
        
        Navigations.Add(new PopupNavigationModel(
            "Add to playlist",
            (PathGeometry)Application.Current.Resources["AddToPlaylistIcon"],
            () =>
            {
                DialogViewModel.OpenGlobal("Add to playlist",
                    new AddToPlaylistViewModel(track.Id, AddToPlaylist.Track));
            }));

        if (options.IsAdmin || options.IsOwner)
            Navigations.Add(new PopupNavigationModel(
                "Change Image",
                (PathGeometry)Application.Current.Resources["ImageIcon"],
                () => { Task.Run(() => App.GetService().ChangeImage(track.Id, ChangeImage.Tracks)); }));

        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
            Navigations.Add(new PopupNavigationModel(
                "Edit genres",
                (PathGeometry)Application.Current.Resources["EditIcon"],
                () => { DialogViewModel.OpenGlobal("Edit genres", new EditTrackGenreViewModel(track.Id)); }));

        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
            Navigations.Add(new PopupNavigationModel(
                "Change audio",
                (PathGeometry)Application.Current.Resources["SelectFileIcon"],
                () => { Task.Run(() => App.GetService().ChangeAudio(track.Id)); }));

        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
            Navigations.Add(new PopupNavigationModel(
                "Add to album",
                (PathGeometry)Application.Current.Resources["AddIcon"],
                () =>
                {
                    DialogViewModel.OpenGlobal("Add to album", new AddToAlbumViewModel(track.Id, ContainerType.Track));
                }));

        if (options.IsActionsAllowed && (options.IsAdmin || options.IsOwner))
            Navigations.Add(new PopupNavigationModel(
                "Add artist",
                (PathGeometry)Application.Current.Resources["AddIcon"],
                () =>
                {
                    DialogViewModel.OpenGlobal("Add artist", new AddArtistViewModel(track.Id, ContainerType.Track));
                }));

        switch (options.RemoveType)
        {
            case RemoveType.Remove:
                if (options.IsAdmin || options.IsOwner || options.IsContainerOwner)
                {
                    if (options is { IsContainerOwner: true, ContainerType: ContainerType.Artist  })
                        Navigations.Add(new PopupNavigationModel(
                            "Delete",
                            (PathGeometry)Application.Current.Resources["TrashIcon"],
                            () => { Task.Run(async () => { await App.GetService().DeleteTrack(track.Id); }); },
                            true));
                    else
                        Navigations.Add(new PopupNavigationModel(
                            "Remove",
                            (PathGeometry)Application.Current.Resources["TrashIcon"],
                            () => { TrackEvents.RequestRemove(track.Id, options.ContainerType); },
                            true));
                }

                break;
            case RemoveType.Delete when options.IsAdmin || options.IsOwner:
                Navigations.Add(new PopupNavigationModel(
                    "Delete",
                    (PathGeometry)Application.Current.Resources["TrashIcon"],
                    () => { Task.Run(async () => { await App.GetService().DeleteTrack(track.Id); }); },
                    true));
                break;
            case RemoveType.Nothing:
                break;
        }
    }

    public override string ModelName { get; protected set; } = nameof(TrackPopupViewModel);
}