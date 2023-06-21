using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Media.Album;
using Music.Shared.DTOs.Requests.Albums;

namespace Music.Applications.Windows.Views.Media.Albums;

public partial class CreateAlbumView
{
    private CreateAlbumViewModel? _model;

    public CreateAlbumView()
    {
        InitializeComponent();
    }

    private void TitleTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        DisplayErrorMessage("");
    }

    private void DisplayErrorMessage(string message)
    {
        if (_model == null) return;
        _model.ErrorMessage = message;
    }

    private void PublicationDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
    {
        DisplayErrorMessage("");
    }

    private void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        DisplayErrorMessage("");
        if (sender is not DatePicker { DataContext: CreateAlbumViewModel model } picker) return;
        picker.SelectedDate ??= DateTime.Now;
        model.PublicationDate = picker.SelectedDate;
    }

    private async void CreateButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not CustomButton { DataContext: CreateAlbumViewModel model }) return;
        _model ??= model;

        if (string.IsNullOrEmpty(model.Title) || model.Title.Length is < 3 or > 40)
        {
            DisplayErrorMessage("Length not in range 3 and 40");
            return;
        }

        await Task.Run(async () =>
        {
            await App.GetService().CreateAlbum(
                new AlbumCreateRequest(model.Title, DateOnly.FromDateTime(model.PublicationDate!.Value)),
                _model.IsArtist ? null : model.ArtistId);
            DialogEvents.Close();
        });
    }
}