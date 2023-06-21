using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Applications.Windows.ViewModels.Navigation;

namespace Music.Applications.Windows.Components;

public partial class NavigationComponent : UserControl
{
    public NavigationComponent()
    {
        InitializeComponent();
    }

    private void PlaylistBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border { DataContext: UserPlaylist playlist }) return;
        ApplicationService.NavigateTo(new MediaPlaylistViewModel(playlist.Id));
    }

    private async void TrashArea_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Viewbox { DataContext: UserPlaylist playlist }) return;

        await Task.Run(async () =>
        {
            var appService = App.GetService();
            await appService.DeletePlaylist(playlist.Id);
        });
    }

    private void HomeButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<HomeViewModel>();
    }

    private void BrowseButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<BrowseViewModel>();
    }

    private void ArtistStudioButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<ArtistStudioViewModel>();
    }

    private void AdminPanelButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<AdminPanelViewModel>();
    }

    private void LikedButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<LikedViewModel>();
    }

    private void TracksButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<TracksViewModel>();
    }

    private void ArtistsButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<ArtistsViewModel>();
    }

    private void AlbumsButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<AlbumsViewModel>();
    }

    private void CreatePlaylistButton_OnClicked(object sender, RoutedEventArgs e)
    {
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();
        model.DialogViewModel.Open("Create playlist", new CreatePlaylistViewModel());
    }

    private void PlaylistsGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }
}