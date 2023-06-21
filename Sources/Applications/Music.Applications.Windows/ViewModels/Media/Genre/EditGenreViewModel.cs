using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.ViewModels.Media.Genre;

public class EditGenreViewModel : ViewModelBase
{
    private string _color;

    private string _errorMessage = "";

    private string _value;

    public EditGenreViewModel(bool isCreating, Guid? genreId = null, string value = "", string color = "#EAE0C8")
    {
        IsCreating = isCreating;
        _color = color;
        _value = value;
        GenreId = genreId ?? Guid.Empty;
    }

    public bool IsCreating { get; }

    public Guid GenreId { get; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string Color
    {
        get => _color;
        set
        {
            var color = value.Length == 7 ? value : value.Remove(1, 2);
            if (color == _color) return;
            _color = color;
            OnPropertyChanged();
        }
    }

    public string Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    public override string ModelName { get; protected set; } = nameof(EditGenreViewModel);
}