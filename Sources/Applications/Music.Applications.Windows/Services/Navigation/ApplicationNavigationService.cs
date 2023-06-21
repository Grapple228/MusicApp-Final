using Music.Applications.Windows.Core;

namespace Music.Applications.Windows.Services.Navigation;

public class ApplicationNavigationService : NavigationServiceBase
{
    public ApplicationNavigationService(Func<Type, ViewModelBase> viewModelFactory) : base(viewModelFactory)
    {
    }
}