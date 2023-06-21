using System.Windows.Controls;
using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.ViewModels.Popup;

public class ChangeUsernameViewModel : ViewModelBase
{
    private string _username;
    public override string ModelName { get; protected set; } = nameof(ChangeUsernameViewModel);

    public ChangeUsernameViewModel(Guid userId, string username)
    {
        UserId = userId;
        Username = username;
    }

    public Guid UserId { get; }

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
}