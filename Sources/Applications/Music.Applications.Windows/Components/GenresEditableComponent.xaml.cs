using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media.Genres;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Media.Genre;

namespace Music.Applications.Windows.Components;

public partial class GenresEditableComponent : UserControl
{
    public GenresEditableComponent()
    {
        InitializeComponent();
    }

    private void ItemBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void DeleteButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Genre genre }) return;
        GenreEvents.Delete(genre.Id);
    }

    private void ColorText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock textBlock) return;
        if (Clipboard.GetText() == textBlock.Text) return;
        Clipboard.SetText(textBlock.Text);
        NotificationEvents.DisplayNotification("Copied to clipboard", "Copied");
    }

    private void EditButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Genre genre }) return;
        DialogViewModel.OpenGlobal("Edit genre", new EditGenreViewModel(false, genre.Id, genre.Value, genre.Color));
    }
}