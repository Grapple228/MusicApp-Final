using Music.Applications.Windows.Core;
using Music.Applications.Windows.Models;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class HomeViewModel : ViewModelBase
{
    public HomeViewModel(CurrentUser user)
    {
        User = user;
    }

    public CurrentUser User { get; }

    public override string ModelName { get; protected set; } = "Home";
}