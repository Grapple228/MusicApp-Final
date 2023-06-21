using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Navigation;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Views.Navigation;

public partial class ArtistStudioView
{
    public ArtistStudioView()
    {
        InitializeComponent();
    }

    private void AlbumsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Albums;
    }

    private void TracksNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Tracks;
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: IOwnerable ownerable } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new ArtistPopupViewModel(ownerable.Id, new NavigationPopupOptions
        {
            RemoveType = RemoveType.Nothing,
            IsOwner = ownerable.IsUserOwner,
            IsSecondaryOwner = ownerable.IsSecondaryOwner,
            IsRemovable = ownerable.IsRemovable
        }, true);

        model.PopupViewModel.ChangeState(
            button,
            navigation
        );
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: ArtistStudioViewModel model}) return;
        PlayerEvents.RequestContainerPlay(model.Artist.Id, ContainerType.Artist);
    }
}