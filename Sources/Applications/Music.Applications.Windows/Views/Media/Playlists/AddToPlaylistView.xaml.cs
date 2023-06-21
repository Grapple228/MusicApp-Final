using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;

namespace Music.Applications.Windows.Views.Media.Playlists;

public partial class AddToPlaylistView
{
    public AddToPlaylistView()
    {
        InitializeComponent();
    }

    private void PlaylistsGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void PlaylistBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border { DataContext: UserPlaylist playlist }) return;
        PlaylistEvents.RequestAddToPlaylist(playlist.Id);
        DialogEvents.Close();
    }
}