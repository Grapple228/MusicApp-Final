using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media;
using Music.Applications.Windows.Models.Media.Playlists;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.User;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.Common;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Components;

public partial class PlaylistsComponent : UserControl
{
    public static readonly DependencyProperty ContainerTypeProperty =
        DependencyProperty.Register(nameof(ContainerType), typeof(ContainerType), typeof(PlaylistsComponent),
            new PropertyMetadata(ContainerType.Track));

    public static readonly DependencyProperty IsReactionProperty =
        DependencyProperty.Register(nameof(IsReaction), typeof(bool), typeof(PlaylistsComponent),
            new PropertyMetadata(true));

    public static readonly DependencyProperty RemoveTypeProperty =
        DependencyProperty.Register(nameof(RemoveType), typeof(RemoveType), typeof(PlaylistsComponent),
            new PropertyMetadata(RemoveType.Delete));

    public static readonly DependencyProperty IsActionsAllowedProperty =
        DependencyProperty.Register(nameof(IsActionsAllowed), typeof(bool), typeof(PlaylistsComponent),
            new PropertyMetadata(true));

    public static readonly DependencyProperty IsContainerOwnerProperty =
        DependencyProperty.Register(nameof(IsContainerOwner), typeof(bool), typeof(PlaylistsComponent),
            new PropertyMetadata(false));

    public PlaylistsComponent()
    {
        InitializeComponent();
    }

    public ContainerType ContainerType
    {
        get => (ContainerType)GetValue(ContainerTypeProperty);
        set => SetValue(ContainerTypeProperty, value);
    }

    public bool IsReaction
    {
        get => (bool)GetValue(IsReactionProperty);
        set => SetValue(IsReactionProperty, value);
    }

    public RemoveType RemoveType
    {
        get => (RemoveType)GetValue(RemoveTypeProperty);
        set => SetValue(RemoveTypeProperty, value);
    }

    public bool IsActionsAllowed
    {
        get => (bool)GetValue(IsActionsAllowedProperty);
        set => SetValue(IsActionsAllowedProperty, value);
    }

    public bool IsContainerOwner
    {
        get => (bool)GetValue(IsContainerOwnerProperty);
        set => SetValue(IsContainerOwnerProperty, value);
    }

    private void PlaylistTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock { DataContext: IModel model }) return;
        ApplicationService.NavigateTo(new MediaPlaylistViewModel(model.Id));
    }

    private void Playlist_OnLiked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaModel model }) return;
        model.ChangeLiked();
    }

    private void Playlist_OnBlocked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: MediaModel model }) return;
        model.ChangeBlocked();
    }

    private void User_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock { DataContext: Playlist playlist }) return;
        if(playlist.Owner == null) return;
        ApplicationService.NavigateTo(new UserViewModel(playlist.Owner.Id));
    }

    private void ItemBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IOwnerable ownerable } element) return;
        var mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

        // TODO вынести в factory 
        var navigation = new PlaylistPopupViewModel(ownerable.Id, new NavigationPopupOptions
        {
            RemoveType = RemoveType,
            IsOwner = ownerable.IsUserOwner,
            IsSecondaryOwner = ownerable.IsSecondaryOwner,
            IsContainerOwner = IsContainerOwner,
            ContainerType = ContainerType,
            IsActionsAllowed = IsActionsAllowed
        });

        mainViewModel.PopupViewModel.ChangeState(
            element,
            navigation
        );
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IPlaylist playlist}) return;
        PlayerEvents.RequestContainerPlay(playlist.Id, ContainerType.Playlist);
    }
}