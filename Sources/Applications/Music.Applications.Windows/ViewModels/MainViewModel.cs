using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services.Navigation;
using Music.Applications.Windows.ViewModels.Authentication;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Popup;

namespace Music.Applications.Windows.ViewModels;

public class MainViewModel : ViewModelBase
{
    private DialogViewModel? _dialogViewModel;
    private PopupViewModel? _popupViewModel;

    public MainViewModel(GlobalNavigationService navigation)
    {
        Navigation = navigation;
    }
    

    public PopupViewModel PopupViewModel
    {
        get { return _popupViewModel ??= App.ServiceProvider.GetRequiredService<PopupViewModel>(); }
    }

    public DialogViewModel DialogViewModel
    {
        get { return _dialogViewModel ??= App.ServiceProvider.GetRequiredService<DialogViewModel>(); }
    }

    public GlobalNavigationService Navigation { get; init; }

    public override string ModelName { get; protected set; } = nameof(MainViewModel);
}