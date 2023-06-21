using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.ViewModels.Authentication;

public abstract class AuthenticationViewModelBase : ViewModelBase
{
    private string _errorMessage = "";
    private string _username = "";

    protected AuthenticationViewModelBase(string username)
    {
        Username = username;
    }

    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public void ChangeErrorMessage(string message)
    {
        ErrorMessage = message;
    }
}