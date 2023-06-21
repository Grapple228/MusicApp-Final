using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media.Artists;

namespace Music.Applications.Windows.Views.Media.Artists;

public partial class AddArtistView
{
    public AddArtistView()
    {
        InitializeComponent();
    }

    private void ArtistGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void ArtistBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Artist artist }) return;
        ArtistEvents.RequestArtistAdd(artist.Id);
        DialogEvents.Close();
    }
}