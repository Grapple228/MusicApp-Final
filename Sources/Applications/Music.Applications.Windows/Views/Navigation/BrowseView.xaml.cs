using System.Windows;
using System.Windows.Controls;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.ViewModels.Navigation;

namespace Music.Applications.Windows.Views.Navigation;

public partial class BrowseView
{
    public BrowseView()
    {
        InitializeComponent();
    }

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is not TextBox{DataContext: BrowseViewModel model} textBox) return;
        model.SearchQuery = textBox.Text;
    }

    private void PlaylistsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Playlists;
    }

    private void ArtistsNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Artists;
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
}