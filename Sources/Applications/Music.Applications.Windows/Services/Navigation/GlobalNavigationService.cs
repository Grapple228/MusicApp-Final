using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.Services.Navigation;

public class GlobalNavigationService : NavigationServiceBase
{
    public GlobalNavigationService(Func<Type, ViewModelBase> viewModelFactory) : base(viewModelFactory)
    {
    }
}