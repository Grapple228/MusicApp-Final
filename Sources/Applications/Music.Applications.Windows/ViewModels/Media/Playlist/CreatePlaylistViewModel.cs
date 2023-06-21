using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.ViewModels.Media.Playlist;

public class CreatePlaylistViewModel : ViewModelBase
{
    private string _errorMessage;
    private bool _isPrivate;
    private string _title;

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

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public bool IsPrivate
    {
        get => _isPrivate;
        set
        {
            if (value == _isPrivate) return;
            _isPrivate = value;
            OnPropertyChanged();
        }
    }

    public override string ModelName { get; protected set; } = nameof(CreatePlaylistViewModel);

    public override bool Equals(object? obj)
    {
        return obj is CreatePlaylistViewModel model
               && model.ModelName == ModelName;
    }
}