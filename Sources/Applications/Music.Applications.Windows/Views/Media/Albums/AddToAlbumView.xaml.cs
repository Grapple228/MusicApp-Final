using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media.Albums;

namespace Music.Applications.Windows.Views.Media.Albums;

public partial class AddToAlbumView
{
    public AddToAlbumView()
    {
        InitializeComponent();
    }

    private void AlbumGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void AlbumBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Album album }) return;
        AlbumEvents.RequestAddToAlbum(album.Id);
        DialogEvents.Close();
    }
}