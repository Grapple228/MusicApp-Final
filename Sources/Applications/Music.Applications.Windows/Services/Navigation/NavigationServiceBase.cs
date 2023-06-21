using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Shared.Common;

namespace Music.Applications.Windows.Services.Navigation;

public delegate void NavigatedHandler(ViewModelBase modelBase);

public abstract class NavigationServiceBase : ObservableObject, IDisposable, INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase? _currentView;

    protected NavigationServiceBase(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        GlobalEvents.UpdateRequested += GlobalEventsOnUpdateRequested;
    }

    public virtual void Dispose()
    {
        _currentView?.Dispose();
        GlobalEvents.UpdateRequested -= GlobalEventsOnUpdateRequested;
        PopupEvents.Close();
        GC.SuppressFinalize(this);
    }

    public ViewModelBase? CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var type = typeof(TViewModel);
        if (CurrentView?.GetType() == type)
        {
            if (CurrentView is LoadableViewModel model) model.Reload();
            return;
        }

        var viewModel = _viewModelFactory.Invoke(type);
        Navigated?.Invoke(viewModel);
        CurrentView?.Dispose();
        CurrentView = viewModel;
        PopupEvents.Close();
    }

    public void SetCurrentView<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        var type = typeof(TViewModel);
        if (CurrentView?.GetType() == type)
            switch (CurrentView)
            {
                case LoadableViewModel loadableViewModel:
                    if (CurrentView is IModel clm && viewModel is IModel lm
                                                  && clm.Id == lm.Id)
                    {
                        Task.Run(async () => await loadableViewModel.Reload());
                        viewModel.Dispose();
                        return;
                    }

                    break;
                case IModel cm when viewModel is IModel m
                                    && cm.Id == m.Id:
                    viewModel.Dispose();
                    return;
            }

        Navigated?.Invoke(viewModel);

        CurrentView?.Dispose();
        CurrentView = viewModel;
        PopupEvents.Close();
    }

    public event NavigatedHandler? Navigated;

    protected virtual void GlobalEventsOnUpdateRequested()
    {
        if (CurrentView is not LoadableViewModel loadableViewModel) return;
        Task.Run(() => loadableViewModel.Reload());
    }

    ~NavigationServiceBase()
    {
        Dispose();
    }
}