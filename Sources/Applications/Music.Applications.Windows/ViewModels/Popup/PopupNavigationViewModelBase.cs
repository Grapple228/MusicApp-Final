using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Models.Popup;

namespace Music.Applications.Windows.ViewModels.Popup;

public abstract class PopupNavigationViewModelBase : ViewModelBase
{
    public ObservableCollection<PopupNavigationModel> Navigations { get; } = new();
}