using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Components;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Views.Media.Playlists;

public partial class MediaPlaylistView
{
    public MediaPlaylistView()
    {
        InitializeComponent();
    }

    private void Playlist_OnLiked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaPlaylistViewModel model }) return;
        model.Playlist.ChangeLiked();
    }

    private void Playlist_OnBlocked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaPlaylistViewModel model }) return;
        model.Playlist.ChangeBlocked();
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: IOwnerable ownerable } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        // TODO вынести в factory 
        var navigation = new PlaylistPopupViewModel(ownerable.Id, new NavigationPopupOptions
        {
            RemoveType = RemoveType.Delete,
            IsOwner = ownerable.IsUserOwner,
            IsSecondaryOwner = ownerable.IsSecondaryOwner,
            IsRemovable = ownerable.IsRemovable
        });

        model.PopupViewModel.ChangeState(
            button,
            navigation
        );
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: MediaPlaylistViewModel model}) return;
        PlayerEvents.RequestContainerPlay(model.Playlist.Id, ContainerType.Playlist);
    }
}