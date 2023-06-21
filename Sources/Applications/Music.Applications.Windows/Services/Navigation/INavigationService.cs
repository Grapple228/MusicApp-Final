using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.Services.Navigation;

public interface INavigationService
{
    ViewModelBase? CurrentView { get; }
    void NavigateTo<T>() where T : ViewModelBase;
    void SetCurrentView<T>(T viewModel) where T : ViewModelBase;
}