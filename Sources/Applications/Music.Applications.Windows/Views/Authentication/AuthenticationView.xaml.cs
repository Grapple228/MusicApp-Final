using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Authentication;
using MusicClient.Exceptions;

namespace Music.Applications.Windows.Views.Authentication;

public partial class AuthenticationView : UserControl
{
    private readonly MainViewModel _model;

    public AuthenticationView()
    {
        InitializeComponent();
        _model = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }

    private void DisplayErrorMessage(string message)
    {
        // TODO ДОБАВИТЬ СЛОВАРЬ С ОШИБКАМИ, ENUM

        if (_model.Navigation.CurrentView is not AuthenticationViewModel signInViewModel) return;
        signInViewModel.ChangeErrorMessage(message);
    }

    private bool CheckData(string username, string password)
    {
        if (username == "")
        {
            DisplayErrorMessage("Username is empty");
            return false;
        }

        if (password == "")
        {
            DisplayErrorMessage("Password is empty");
            return false;
        }

        if (username.Contains(" "))
        {
            DisplayErrorMessage("Username can't contain white spaces!");
            return false;
        }

        if (password.Contains(" "))
        {
            DisplayErrorMessage("Password can't contain white spaces!");
            return false;
        }

        return true;
    }

    private async void TryAuthenticate()
    {
        var username = UsernameText.Text;
        var password = PasswordText.Box.Password;

        if (!CheckData(username, password))
            return;

        try
        {
            await App.GetService().Authenticate(username, password);
        }
        catch (InvalidAuthorizationDataException)
        {
            DisplayErrorMessage("Invalid login or password!");
        }
        catch (ServerUnavailableException)
        {
            DisplayErrorMessage("Server is unavailable");
        }
        catch (Exception)
        {
            DisplayErrorMessage("Unexpected error");
        }
    }

    private void SignUpTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var signUpModel = new RegistrationViewModel(UsernameText.Text);
        _model.Navigation.SetCurrentView(signUpModel);
    }

    private void UsernameText_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        DisplayErrorMessage("");
        if (e.Key == Key.Enter)
            TryAuthenticate();
    }

    private void PasswordText_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        DisplayErrorMessage("");
        if (e.Key == Key.Enter)
            TryAuthenticate();
    }

    private void SignInButton_OnClicked(object sender, RoutedEventArgs e)
    {
        DisplayErrorMessage("");
        TryAuthenticate();
    }
}