using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Artist;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Media.Track;
using Music.Shared.Common;
using MusicClient.Api.Playlists;

namespace Music.Applications.Windows.Components;

public partial class ControlPanelComponent
{
    public ControlPanelComponent()
    {
        InitializeComponent();
    }

    private void TrackTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if(model.TrackId == Guid.Empty) return;
        ApplicationService.NavigateTo(new MediaTrackViewModel(model.TrackId));
    }

    private void ReactionComponent_OnLiked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: ControlPanelViewModel model }) return;
        model.CurrentTrack?.ChangeLiked();
    }

    private void ReactionComponent_OnBlocked(object sender, RoutedEventArgs e)
    {
        if (sender is not ReactionComponent { DataContext: ControlPanelViewModel model }) return;
        model.CurrentTrack?.ChangeBlocked();
    }

    private void ArtistTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IModel model }) return;
        ApplicationService.NavigateTo(new MediaArtistViewModel(model.Id));
    }

    private void PositionSlider_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not Slider { DataContext: ControlPanelViewModel control } slider) return;
        var position = e.GetPosition(slider).X;
        control.TimeOffset = position;
        var time = (int)(slider.Maximum / slider.ActualWidth * position);
        control.CursorTime = time > control.Player?.MaximumPosition
            ? control.Player.MaximumPosition
            : time < 0
                ? 0
                : time;
    }
    
    
    private void VolumeButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        model.Player?.ChangeMute();
    }

    private void ShuffleButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if (model.Player == null) return;
        model.Player.ShuffleType = model.Player.ShuffleType.Next();
    }

    private void RepeatButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if (model.Player == null) return;
        model.Player.RepeatType = model.Player.RepeatType.Next();
    }

    private void RepeatButton_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if (model.Player == null) return;
        model.Player.RepeatType = model.Player.RepeatType.Previous();
    }

    private void AddToPlaylistButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if (model.CurrentTrack == null) return;
        DialogViewModel.OpenGlobal("Add to playlist",
            new AddToPlaylistViewModel(model.CurrentTrack.Id, AddToPlaylist.Track));
    }

    private void PlayButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: ControlPanelViewModel model }) return;
        if(model.ListenQuery.Count == 0) return;
        
        PlayerEvents.RequestTrackPlay(model.CurrentTrack?.Id ?? model.ListenQuery.First().Id, model.ContainerId, model.ContainerType);
    }

    private async void NextButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: ControlPanelViewModel controlPanelViewModel}) return;
        await controlPanelViewModel.NextTrack();
    }

    private async void PrevButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: ControlPanelViewModel controlPanelViewModel}) return;
        await controlPanelViewModel.PrevTrack();
    }

    private void PlayingImageComponent_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: Track track}) return;
        var model = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEvents.RequestTrackPlay(track.Id, model.ContainerId, model.ContainerType);
    }

    private void QueryTrackTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IModel model}) return;
        ApplicationService.NavigateTo(new MediaTrackViewModel(model.Id));
    }

    private void QueryArtistTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IModel model}) return;
        ApplicationService.NavigateTo(new MediaArtistViewModel(model.Id));
    }

    private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ApplicationService.OnPreviewMouseWheel(sender, e);
    }
}