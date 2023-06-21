using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Media.Playlist;
using Music.Shared.DTOs.Requests.Playlists;

namespace Music.Applications.Windows.Views.Media.Playlists;

public partial class CreatePlaylistView
{
    private CreatePlaylistViewModel? _model;

    public CreatePlaylistView()
    {
        InitializeComponent();
    }

    private async void CreateButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not CustomButton { DataContext: CreatePlaylistViewModel model }) return;

        _model ??= model;

        if (string.IsNullOrEmpty(model.Title) || model.Title.Length is < 3 or > 40)
        {
            DisplayErrorMessage("Length not in range 3 and 40");
            return;
        }

        await Task.Run(async () =>
        {
            await App.GetService().CreatePlaylist(new PlaylistCreateRequest(model.Title!, !model.IsPrivate));
            DialogEvents.Close();
        });
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
}