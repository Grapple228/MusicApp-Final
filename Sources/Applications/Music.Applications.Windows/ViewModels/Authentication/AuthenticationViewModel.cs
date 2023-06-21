namespace Music.Applications.Windows.ViewModels.Authentication;

public class AuthenticationViewModel : AuthenticationViewModelBase
{
    public AuthenticationViewModel() : base("")
    {
    }

    public AuthenticationViewModel(string? username) : base(username ?? "")
    {
    }

    public bool IsRemember { get; set; }

    public override string ModelName { get; protected set; } = nameof(AuthenticationViewModel);
}