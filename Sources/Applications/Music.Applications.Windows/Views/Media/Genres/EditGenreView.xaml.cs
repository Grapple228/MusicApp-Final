using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Media.Genre;
using Music.Shared.DTOs.Requests.Genres;

namespace Music.Applications.Windows.Views.Media.Genres;

public partial class EditGenreView
{
    private EditGenreViewModel? _model;

    public EditGenreView()
    {
        InitializeComponent();
    }

    private void CreateButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not CustomButton { DataContext: EditGenreViewModel model }) return;
        _model ??= model;

        if (string.IsNullOrEmpty(model.Value) || model.Value.Length is < 3 or > 40)
        {
            DisplayErrorMessage("Length not in range 3 and 40");
            return;
        }

        if (model.IsCreating)
            GenreEvents.Create(new GenreCreateRequest(model.Value, model.Color));
        else
            GenreEvents.Update(model.GenreId, new GenreUpdateRequest(model.Value, model.Color));


        DialogEvents.Close();
    }

    private void DisplayErrorMessage(string message)
    {
        if (_model == null) return;
        _model.ErrorMessage = message;
    }

    private void ValueTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        DisplayErrorMessage("");
    }
}