using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Artist;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.Common;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Components;

public partial class ArtistsComponent : UserControl
{
    public static readonly DependencyProperty ContainerTypeProperty =
        DependencyProperty.Register(nameof(ContainerType), typeof(ContainerType), typeof(ArtistsComponent),
            new PropertyMetadata(ContainerType.Track));

    public static readonly DependencyProperty RemoveTypeProperty =
        DependencyProperty.Register(nameof(RemoveType), typeof(RemoveType), typeof(ArtistsComponent),
            new PropertyMetadata(RemoveType.Delete));

    public static readonly DependencyProperty IsContainerOwnerProperty =
        DependencyProperty.Register(nameof(IsContainerOwner), typeof(bool), typeof(ArtistsComponent),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsActionsAllowedProperty =
        DependencyProperty.Register(nameof(IsActionsAllowed), typeof(bool), typeof(ArtistsComponent),
            new PropertyMetadata(true));

    public static readonly DependencyProperty IsReactionProperty =
        DependencyProperty.Register(nameof(IsReaction), typeof(bool), typeof(ArtistsComponent),
            new PropertyMetadata(true));

    public ArtistsComponent()
    {
        InitializeComponent();
    }

    public ContainerType ContainerType
    {
        get => (ContainerType)GetValue(ContainerTypeProperty);
        set => SetValue(ContainerTypeProperty, value);
    }

    public RemoveType RemoveType
    {
        get => (RemoveType)GetValue(RemoveTypeProperty);
        set => SetValue(RemoveTypeProperty, value);
    }

    public bool IsContainerOwner
    {
        get => (bool)GetValue(IsContainerOwnerProperty);
        set => SetValue(IsContainerOwnerProperty, value);
    }

    public bool IsActionsAllowed
    {
        get => (bool)GetValue(IsActionsAllowedProperty);
        set => SetValue(IsActionsAllowedProperty, value);
    }

    public bool IsReaction
    {
        get => (bool)GetValue(IsReactionProperty);
        set => SetValue(IsReactionProperty, value);
    }

    private void ArtistName_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock { DataContext: IModel model }) return;
        ApplicationService.NavigateTo(new MediaArtistViewModel(model.Id));
    }

    private void Artist_OnLiked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ReactionComponent { DataContext: MediaModel model }) return;
        model.ChangeLiked();
    }

    private void Artist_OnBlocked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ReactionComponent { DataContext: MediaModel model }) return;
        model.ChangeBlocked();
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IOwnerable ownerable } element) return;
        var mainModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new ArtistPopupViewModel(ownerable.Id,
            new NavigationPopupOptions
            {
                RemoveType = RemoveType,
                IsOwner = ownerable.IsUserOwner,
                IsSecondaryOwner = ownerable.IsSecondaryOwner,
                IsContainerOwner = IsContainerOwner,
                IsRemovable = ownerable.IsRemovable,
                ContainerType = ContainerType,
                IsActionsAllowed = IsActionsAllowed
            }, false);

        mainModel.PopupViewModel.ChangeState(
            element,
            navigation
        );
    }

    private void ItemBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IArtist artist}) return;
        PlayerEvents.RequestContainerPlay(artist.Id, ContainerType.Artist);
    }
}