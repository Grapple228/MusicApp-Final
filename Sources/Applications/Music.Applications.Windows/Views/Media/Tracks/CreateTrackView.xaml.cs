using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Media.Track;

namespace Music.Applications.Windows.Views.Media.Tracks;

public partial class CreateTrackView
{
    private CreateTrackViewModel? _model;

    public CreateTrackView()
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

    private async void CreateButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not CustomButton { DataContext: CreateTrackViewModel model }) return;
        _model ??= model;

        if (string.IsNullOrEmpty(model.Title) || model.Title.Length is < 3 or > 40)
        {
            DisplayErrorMessage("Length not in range 3 and 40");
            return;
        }

        if (string.IsNullOrEmpty(model.Filename))
        {
            DisplayErrorMessage("File not selected");
            return;
        }

        await Task.Run(async () =>
        {
            await App.GetService().CreateTrack(model);
            DialogEvents.Close();
        });
    }

    private void OpenFileDialog(object sender)
    {
        if (sender is not FrameworkElement { DataContext: CreateTrackViewModel model }) return;
        DisplayErrorMessage("");

        var ofd = new OpenFileDialog
        {
            Filter = "Audio (*.mp3) | *.mp3",
            Title = "Load audio"
        };
        var result = ofd.ShowDialog();
        if (!result.HasValue || !result.Value) return;

        model.Filename = ofd.FileName;
    }

    private void SelectFileButton_OnClicked(object sender, RoutedEventArgs e)
    {
        OpenFileDialog(sender);
    }

    private void FilenameTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        OpenFileDialog(sender);
    }

    private void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        DisplayErrorMessage("");
        if (sender is not DatePicker { DataContext: CreateTrackViewModel model } picker) return;
        picker.SelectedDate ??= DateTime.Now;
        model.PublicationDate = picker.SelectedDate;
    }

    private void PublicationDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
    {
        DisplayErrorMessage("");
    }
}