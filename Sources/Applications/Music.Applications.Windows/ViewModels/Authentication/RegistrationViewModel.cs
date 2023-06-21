namespace Music.Applications.Windows.ViewModels.Authentication;

public class RegistrationViewModel : AuthenticationViewModelBase
{
    public RegistrationViewModel() : base("")
    {
    }

    public RegistrationViewModel(string username) : base(username)
    {
    }

    public override string ModelName { get; protected set; } = nameof(RegistrationViewModel);
}