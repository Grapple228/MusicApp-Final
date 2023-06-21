using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Components;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Views.Media.Tracks;

public partial class MediaTrackView
{
    public MediaTrackView()
    {
        InitializeComponent();
    }

    private void Track_OnBlocked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaTrackViewModel model }) return;
        model.Track.ChangeBlocked();
    }

    private void Track_OnLiked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaTrackViewModel model }) return;
        model.Track.ChangeLiked();
    }

    private void AlbumsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Albums;
    }

    private void ArtistsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Artists;
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: MediaTrackViewModel track } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new TrackPopupViewModel(track.Track,
            new NavigationPopupOptions
            {
                RemoveType = RemoveType.Delete,
                IsOwner = track.Track.IsUserOwner,
                IsSecondaryOwner = track.Track.IsSecondaryOwner,
                IsRemovable = track.Track.IsRemovable,
                ContainerType = ContainerType.Track
            });

        model.PopupViewModel.ChangeState(
            button,
            navigation
        );
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: MediaTrackViewModel model}) return;
        PlayerEvents.RequestContainerPlay(model.Track.Id, ContainerType.Track);
    }
}