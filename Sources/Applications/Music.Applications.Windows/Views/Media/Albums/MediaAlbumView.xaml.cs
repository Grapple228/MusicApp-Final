using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Components;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Views.Media.Albums;

public partial class MediaAlbumView
{
    public MediaAlbumView()
    {
        InitializeComponent();
    }

    private void Album_OnLiked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ReactionComponent { DataContext: MediaAlbumViewModel model }) return;
        model.Album.ChangeLiked();
    }

    private void Album_OnBlocked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ReactionComponent { DataContext: MediaAlbumViewModel model }) return;
        model.Album.ChangeBlocked();
    }

    private void TracksNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Tracks;
    }

    private void ArtistsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Artists;
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: IOwnerable ownerable } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new AlbumPopupViewModel(ownerable.Id,
            new NavigationPopupOptions
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
        if(sender is not FrameworkElement{DataContext: MediaAlbumViewModel model}) return;
        PlayerEvents.RequestContainerPlay(model.Album.Id, ContainerType.Album);
        
    }
}