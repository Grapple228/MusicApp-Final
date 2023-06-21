using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.Components;

public partial class GenreSelectorComponent : UserControl
{
    public GenreSelectorComponent()
    {
        InitializeComponent();
    }

    private void NotSelectedGenre_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: GenreDto genreDto }) return;
        GenreEvents.Add(genreDto);
    }

    private void SelectedGenre_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: GenreDto genreDto }) return;
        GenreEvents.Remove(genreDto);
    }
}